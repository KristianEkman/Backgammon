using Backend.Dto;
using Backend.Dto.Actions;
using Backend.Dto.toplist;
using Backend.Rules;
using Microsoft.Extensions.Logging;
using Rules;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class GameManager
    {
        // Needed for serialization
        public GameManager()
        {

        }

        internal GameManager(ILogger<GameManager> logger)
        {
            Game = Game.Create();
            Created = DateTime.Now;
            Logger = logger;
        }

        internal WebSocket Client1 { get; set; }
        internal WebSocket Client2 { get; set; }
        public Game Game { get; set; }
        public DateTime Created { get; private set; }
        internal bool SearchingOpponent { get; set; }
        public string Inviter { get; set; }
        internal ILogger<GameManager> Logger { get; set; }
        internal event EventHandler Ended;

        CancellationTokenSource moveTimeOut = new CancellationTokenSource();

        private void StartGame()
        {
            Game.ThinkStart = DateTime.Now;
            var gameDto = Game.ToDto();
            var action = new GameCreatedActionDto
            {
                game = gameDto
            };
            action.myColor = PlayerColor.black;
            _ = Send(Client1, action);
            action.myColor = PlayerColor.white;
            _ = Send(Client2, action);
            Game.PlayState = Game.State.FirstThrow;
            // todo: visa på clienten även när det blir samma 
            while (Game.PlayState == Game.State.FirstThrow)
            {
                Game.RollDice();
                var rollAction = new DicesRolledActionDto
                {
                    dices = Game.Roll.Select(d => d.ToDto()).ToArray(),
                    playerToMove = (PlayerColor)Game.CurrentPlayer,
                    validMoves = Game.ValidMoves.Select(m => m.ToDto()).ToArray(),
                    moveTimer = Game.ClientCountDown
                };
                _ = Send(Client1, rollAction);
                _ = Send(Client2, rollAction);
            }
            moveTimeOut = new CancellationTokenSource();
            _ = Utils.RepeatEvery(500, () =>
            {
                TimeTick();
            }, moveTimeOut);
        }

        private void TimeTick()
        {
            if (!moveTimeOut.IsCancellationRequested)
            {
                var ellapsed = DateTime.Now - Game.ThinkStart;
                if (ellapsed.TotalSeconds > Game.TotalThinkTime)
                {
                    Logger.LogInformation($"The time run out for {Game.CurrentPlayer}");
                    var winner = Game.CurrentPlayer == Player.Color.Black ? PlayerColor.white : PlayerColor.black;
                    _ = EndGame(winner);
                }
            }
        }

        private async Task EndGame(PlayerColor winner)
        {
            moveTimeOut.Cancel();
            Game.PlayState = Game.State.Ended;
            Logger.LogInformation($"The winner is ${winner}");
            var newScore = SaveWinner(winner);
            await SendWinner(winner, newScore);
            Ended?.Invoke(this, EventArgs.Empty);
        }

        private void SendNewRoll()
        {
            Game.RollDice();
            var rollAction = new DicesRolledActionDto
            {
                dices = Game.Roll.Select(d => d.ToDto()).ToArray(),
                playerToMove = (PlayerColor)Game.CurrentPlayer,
                validMoves = Game.ValidMoves.Select(m => m.ToDto()).ToArray(),
                moveTimer = Game.ClientCountDown
            };
            _ = Send(Client1, rollAction);
            _ = Send(Client2, rollAction);
        }

        internal async Task ConnectAndListen(WebSocket webSocket, Player.Color color, Db.User dbUser)
        {
            if (color == Player.Color.Black)
            {
                Client1 = webSocket;
                Game.BlackPlayer.Id = dbUser != null ? dbUser.Id : Guid.Empty;
                Game.BlackPlayer.Name = dbUser != null ? dbUser.Name : "Guest";
                await ListenOn(webSocket);
            }
            else
            {
                Client2 = webSocket;
                Game.WhitePlayer.Id = dbUser != null ? dbUser.Id : Guid.Empty;
                Game.WhitePlayer.Name = dbUser != null ? dbUser.Name : "Guest";
                CreateDbGame();
                StartGame();
                await ListenOn(webSocket);
            }
        }

        private void CreateDbGame()
        {
            using (var db = new Db.BgDbContext())
            {
                var blackUser = db.Users.Single(u => u.Id == Game.BlackPlayer.Id);
                var black = new Db.Player
                {
                    Id = Guid.NewGuid(), // A player is not the same as a user.
                    Color = Db.Color.Black,
                    User = blackUser
                };
                blackUser.Players.Add(black);

                var whiteUser = db.Users.Single(u => u.Id == Game.WhitePlayer.Id);
                var white = new Db.Player
                {
                    Id = Guid.NewGuid(),
                    Color = Db.Color.White,
                    User = whiteUser
                };
                whiteUser.Players.Add(white);

                var game = new Db.Game
                {
                    Id = Game.Id,
                    Started = DateTime.Now,
                };

                black.Game = game;
                white.Game = game;

                game.Players.Add(black);
                game.Players.Add(white);
                db.Games.Add(game);
                db.SaveChanges();
            }
        }

        private async Task<string> ReceiveText(WebSocket socket)
        {
            var buffer = new byte[512];
            var sb = new StringBuilder();
            WebSocketReceiveResult result = null;
            while (result == null || (!result.EndOfMessage && !result.CloseStatus.HasValue))
            {
                try
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var text = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                    sb.Append(text);
                }
                catch (Exception exc)
                {
                    Logger.LogInformation($"Cant receive data from socket. Error: {exc.ToString()}");
                    return "";
                }
            }
            return sb.ToString();
        }

        internal async Task Restore(PlayerColor color, WebSocket socket)
        {
            var gameDto = Game.ToDto();
            var restoreAction = new GameRestoreActionDto
            {
                game = gameDto,
                color = color,
                dices = Game.Roll.Select(r => r.ToDto()).ToArray()
            };

            WebSocket otherSocket;
            if (color == PlayerColor.black)
            {
                Client1 = socket;
                otherSocket = Client2;
            }
            else
            {
                Client2 = socket;
                otherSocket = Client1;
            }

            await Send(socket, restoreAction);
            //Also send the state to the other client in case it has made moves.
            if (otherSocket != null && otherSocket.State == WebSocketState.Open)
            {
                restoreAction.color = color == PlayerColor.black ? PlayerColor.white : PlayerColor.black;
                await Send(otherSocket, restoreAction);
            }
            else
            {
                Logger.LogWarning("Failed to send restore to other client");
            }

            await ListenOn(socket);
        }

        private async Task ListenOn(WebSocket socket)
        {
            while (socket.State != WebSocketState.Closed &&
                socket.State != WebSocketState.Aborted &&
                socket.State != WebSocketState.CloseReceived)
            {
                var text = await ReceiveText(socket);
                if (text != null && text.Length > 0)
                {
                    Logger.LogInformation($"Received: {text}");
                    var action = (ActionDto)JsonSerializer.Deserialize(text, typeof(ActionDto));
                    var otherClient = socket == Client1 ? Client2 : Client1;
                    await DoAction(action.actionName, text, socket, otherClient);
                }
            }
        }

        private async Task DoAction(ActionNames actionName, string actionText, WebSocket socket, WebSocket otherSocket)
        {
            Logger.LogInformation($"Doing action: {actionName}");
            if (actionName == ActionNames.movesMade)
            {
                Game.ThinkStart = DateTime.Now;
                var action = (MovesMadeActionDto)JsonSerializer.Deserialize(actionText, typeof(MovesMadeActionDto));
                if (socket == Client1)
                    Game.BlackPlayer.FirstMoveMade = true;
                else
                    Game.WhitePlayer.FirstMoveMade = true;

                DoMoves(action);
                PlayerColor? winner = GetWinner();
                if (winner.HasValue)
                    _ = EndGame(winner.Value);
                else
                    SendNewRoll();
            }
            else if (actionName == ActionNames.opponentMove)
            {
                var action = (OpponentMoveActionDto)JsonSerializer.Deserialize(actionText, typeof(OpponentMoveActionDto));
                _ = Send(otherSocket, action);
            }
            else if (actionName == ActionNames.undoMove)
            {
                var action = (UndoActionDto)JsonSerializer.Deserialize(actionText, typeof(UndoActionDto));
                _ = Send(otherSocket, action);
            }
            else if (actionName == ActionNames.connectionInfo)
            {
                var action = (ConnectionInfoActionDto)JsonSerializer.Deserialize(actionText, typeof(ConnectionInfoActionDto));
                _ = Send(otherSocket, action);
            }
            else if (actionName == ActionNames.resign)
            {
                var winner = Client1 == otherSocket ? PlayerColor.black : PlayerColor.white;
                _ = Resign(winner);
            }
            else if (actionName == ActionNames.exitGame)
            {
                _ = CloseConnections(socket);
            }
        }

        private (NewScoreDto black, NewScoreDto white)? SaveWinner(PlayerColor color)
        {
            if (Game.BlackPlayer.IsGuest() || Game.WhitePlayer.IsGuest())
                return null;

            if (!Game.BlackPlayer.FirstMoveMade || !Game.WhitePlayer.FirstMoveMade)
                return null;
            using (var db = new Db.BgDbContext())
            {
                var dbGame = db.Games.Single(g => g.Id == this.Game.Id);
                if (dbGame.Winner.HasValue) // extra safety
                    return(null, null);

                var black = db.Users.Single(u => u.Id == Game.BlackPlayer.Id);
                var white = db.Users.Single(u => u.Id == Game.WhitePlayer.Id);
                var computed = Score.NewScore(black.Elo, white.Elo, black.GameCount, white.GameCount, color == PlayerColor.black);
                var blackInc = computed.black - black.Elo;
                var whiteInc = computed.white - white.Elo;

                black.GameCount++;
                white.GameCount++;

                black.Elo = computed.black;
                white.Elo = computed.white;

                dbGame.Winner = color;
                db.SaveChanges();

                return (
                    new NewScoreDto { score = black.Elo, increase = blackInc },
                    new NewScoreDto { score = white.Elo, increase = whiteInc });
            }
        }

        private async Task Resign(PlayerColor winner)
        {
            await EndGame(winner);
            Logger.LogInformation($"{winner} won game Game {Game.Id} by resignition.");
        }

        private async Task CloseConnections(WebSocket socket)
        {
            if (socket != null)
            {
                Logger.LogInformation("Closing client");
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Game aborted by client", CancellationToken.None);
                if (socket != null)
                    socket.Dispose();
            }
        }

        private PlayerColor? GetWinner()
        {
            PlayerColor? winner = null;
            if (Game.CurrentPlayer == Player.Color.Black)
            {
                Game.CurrentPlayer = Player.Color.White;
                if (Game.GetHome(Player.Color.Black).Checkers.Count == 15)
                {
                    Game.PlayState = Game.State.Ended;
                    winner = PlayerColor.black;
                }
            }
            else
            {
                Game.CurrentPlayer = Player.Color.Black;
                if (Game.GetHome(Player.Color.White).Checkers.Count == 15)
                {
                    Game.PlayState = Game.State.Ended;
                    winner = PlayerColor.white;
                }
            }

            return winner;
        }

        private void DoMoves(MovesMadeActionDto action)
        {
            if (action.moves == null || action.moves.Length == 0)
                return;

            var firstMove = action.moves[0].ToMove(Game);
            var validMove = Game.ValidMoves.SingleOrDefault(m => firstMove.Equals(m));

            for (int i = 0; i < action.moves.Length; i++)
            {
                var moveDto = action.moves[i];
                if (validMove == null)
                {
                    // Preventing invalid moves to enter the state. Should not happen unless someones hacking the socket or serious bugs.
                    throw new ApplicationException("An attempt ta make an invalid move was made");
                }
                else if (i < action.moves.Length - 1)
                {
                    var nextMove = action.moves[i + 1].ToMove(Game);
                    // Going up the valid moves tree one step for every sent move.
                    validMove = validMove.NextMoves.SingleOrDefault(m => nextMove.Equals(m));
                }

                var color = (Player.Color)moveDto.color;
                var move = moveDto.ToMove(Game);
                Game.MakeMove(move);
            }
        }

        private async Task SendWinner(PlayerColor color, (NewScoreDto black, NewScoreDto white)? newScore)
        {
            var game = Game.ToDto();
            game.winner = color;
            var gameEndedAction = new GameEndedActionDto
            {
                game = game
            };
            gameEndedAction.newScore = newScore?.black;
            await Send(Client1, gameEndedAction);

            gameEndedAction.newScore = newScore?.white;
            await Send(Client2, gameEndedAction);
        }

        internal async Task Send<T>(WebSocket socket, T obj)
        {
            if (socket == null || socket.State != WebSocketState.Open)
            {
                Logger.LogInformation("Cannot send to socket, connection was lost.");
                return;
            }
            var json = JsonSerializer.Serialize<object>(obj);
            Logger.LogInformation($"Sending to client ${json}");
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            try
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception exc)
            {
                Logger.LogError($"Failed to send socket data. Exception: {exc}");
            }
        }

    }
}

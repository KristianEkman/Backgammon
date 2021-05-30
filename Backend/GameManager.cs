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

        internal Ai.Engine Engine = null;
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
                    moveTimeOut.Cancel();
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
            if (!IsAi(Game.BlackPlayer))
                _ = Send(Client1, rollAction);
            if (!IsAi(Game.WhitePlayer))
                _ = Send(Client2, rollAction);
        }

        private bool IsAi(Player player)
        {
            return player.Id.ToString().Equals(Db.User.AiUser, StringComparison.OrdinalIgnoreCase);
        }

        internal async Task ConnectAndListen(WebSocket webSocket, Player.Color color, Db.User dbUser, bool playAi)
        {
            if (color == Player.Color.Black)
            {
                Client1 = webSocket;
                Game.BlackPlayer.Id = dbUser != null ? dbUser.Id : Guid.Empty;
                Game.BlackPlayer.Name = dbUser != null ? dbUser.Name : "Guest";
                Game.BlackPlayer.Photo = dbUser != null ? dbUser.PhotoUrl : "";
                Game.BlackPlayer.Elo = dbUser != null ? dbUser.Elo : 0;
                Game.BlackPlayer.Gold = dbUser != null ? dbUser.Gold - firstBet : 0;
                Game.Stake = firstBet * 2;
                if (playAi)
                {
                    var aiUser = Db.BgDbContext.GetDbUser(Db.User.AiUser);
                    Game.WhitePlayer.Id = aiUser.Id;
                    Game.WhitePlayer.Name = aiUser.Name;
                    // TODO
                    Game.WhitePlayer.Photo = "";
                    Game.WhitePlayer.Elo = dbUser.Elo;
                    Game.WhitePlayer.Gold = dbUser.Gold;
                    Engine = new Ai.Engine(Game);
                    CreateDbGame();
                    StartGame();
                    if (Game.CurrentPlayer == Player.Color.White)
                        await EnginMoves(Client1);
                }
                await ListenOn(webSocket);
            }
            else
            {
                if (playAi)
                    throw new ApplicationException("Ai always playes as white. This is not expected");
                Client2 = webSocket;
                Game.WhitePlayer.Id = dbUser != null ? dbUser.Id : Guid.Empty;
                Game.WhitePlayer.Name = dbUser != null ? dbUser.Name : "Guest";
                Game.WhitePlayer.Photo = dbUser != null ? dbUser.PhotoUrl : "";
                Game.WhitePlayer.Elo = dbUser != null ? dbUser.Elo : 0;
                Game.WhitePlayer.Gold = dbUser != null ? dbUser.Gold - firstBet : 0;
                CreateDbGame();
                StartGame();
                await ListenOn(webSocket);
            }
        }

        const int firstBet = 50;

        private void CreateDbGame()
        {
            using (var db = new Db.BgDbContext())
            {
                var blackUser = db.Users.Single(u => u.Id == Game.BlackPlayer.Id);
                if (blackUser.Gold < firstBet)
                    throw new ApplicationException("Black player dont have enough gold"); // Should be guarder earlier
                blackUser.Gold -= firstBet;
                var black = new Db.Player
                {
                    Id = Guid.NewGuid(), // A player is not the same as a user.
                    Color = Db.Color.Black,
                    User = blackUser,
                };
                blackUser.Players.Add(black);

                var whiteUser = db.Users.Single(u => u.Id == Game.WhitePlayer.Id);
                if (whiteUser.Gold < firstBet)
                    throw new ApplicationException("White player dont have enough gold"); // Should be guarder earlier
                whiteUser.Gold -= firstBet;
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
                await NewTurn(socket);

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
            else if (actionName == ActionNames.rolled)
            {
                var action = (ActionDto)JsonSerializer.Deserialize(actionText, typeof(ActionDto));
                _ = Send(otherSocket, action);
            }
            else if (actionName == ActionNames.requestedDoubling)
            {
                var action = (DoublingActionDto)JsonSerializer.Deserialize(actionText, typeof(DoublingActionDto));
                action.moveTimer = Game.ClientCountDown;

                Game.ThinkStart = DateTime.Now;
                Game.SwitchPlayer();
                _ = Send(otherSocket, action);
            }
            else if (actionName == ActionNames.acceptedDoubling)
            {
                var action = (DoublingActionDto)JsonSerializer.Deserialize(actionText, typeof(DoublingActionDto));
                action.moveTimer = Game.ClientCountDown;
                Game.ThinkStart = DateTime.Now;
                Game.GoldMultiplier *= 2;
                Game.BlackPlayer.Gold -= Game.Stake / 2;
                Game.WhitePlayer.Gold -= Game.Stake / 2;
                
                if (Game.WhitePlayer.Gold < 0 || Game.BlackPlayer.Gold < 0)
                    throw new ApplicationException("Player out of gold. Should not be allowd.");
                
                using (var db = new Db.BgDbContext())
                {
                    var black = db.Users.Single(u => Game.BlackPlayer.Id == u.Id);
                    var white = db.Users.Single(u => Game.WhitePlayer.Id == u.Id);
                    black.Gold = Game.BlackPlayer.Gold;
                    white.Gold = Game.WhitePlayer.Gold;
                    db.SaveChanges();
                }

                Game.Stake += Game.Stake;
                Game.LastDoubler = Game.CurrentPlayer;

                Game.SwitchPlayer();
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

        private async Task NewTurn(WebSocket socket)
        {
            PlayerColor? winner = GetWinner();
            Game.SwitchPlayer();
            if (winner.HasValue)
                _ = EndGame(winner.Value);
            else
            {
                SendNewRoll();
                var plyr = Game.CurrentPlayer == Player.Color.Black ? Game.BlackPlayer : Game.WhitePlayer;
                if (IsAi(plyr))
                    await EnginMoves(socket);
            }
        }

        private async Task EnginMoves(WebSocket client)
        {
            var moves = Engine.GetBestMoves();
            var noMoves = true;
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                if (move == null)
                    continue;
                await Task.Delay(1500);
                var moveDto = move.ToDto();
                moveDto.animate = true;
                var dto = new OpponentMoveActionDto
                {
                    move = moveDto,
                };
                Game.MakeMove(move);
                if (Game.CurrentPlayer == Player.Color.Black)
                    Game.BlackPlayer.FirstMoveMade = true;
                else
                    Game.WhitePlayer.FirstMoveMade = true;

                noMoves = false;
                await Send(client, dto);
            }
            if (noMoves)
                await Task.Delay(4000); // if turn is switch right away, ui will not have time to display dice.
            await NewTurn(client);
        }

        public object StakeLock = new object();
        private (NewScoreDto black, NewScoreDto white)? SaveWinner(PlayerColor color)
        {
            if (Game.BlackPlayer.IsGuest() || Game.WhitePlayer.IsGuest())
                return null;

            if (!Game.BlackPlayer.FirstMoveMade || !Game.WhitePlayer.FirstMoveMade)
            {
                using (var db = new Db.BgDbContext())
                {
                    var black = db.Users.Single(u => u.Id == Game.BlackPlayer.Id);
                    var white = db.Users.Single(u => u.Id == Game.WhitePlayer.Id);
                    black.Gold += Game.Stake / 2;
                    white.Gold += Game.Stake / 2;
                    db.SaveChanges();
                }
                return null; // todo: return stakes
            }

            using (var db = new Db.BgDbContext())
            {
                var dbGame = db.Games.Single(g => g.Id == this.Game.Id);
                if (dbGame.Winner.HasValue) // extra safety
                    return (null, null);

                var black = db.Users.Single(u => u.Id == Game.BlackPlayer.Id);
                var white = db.Users.Single(u => u.Id == Game.WhitePlayer.Id);
                var computed = Score.NewScore(black.Elo, white.Elo, black.GameCount, white.GameCount, color == PlayerColor.black);
                var blackInc = computed.black - black.Elo;
                var whiteInc = computed.white - white.Elo;

                black.GameCount++;
                white.GameCount++;

                black.Elo = computed.black;
                white.Elo = computed.white;

                lock (StakeLock) // Preventing other thread to do the same transaction.
                {
                    Logger.LogInformation("Locked " + Thread.CurrentThread.ManagedThreadId);
                    var stake = Game.Stake;
                    Game.Stake = 0;
                    Logger.LogInformation("Stake" + stake);
                    Logger.LogInformation($"Initial gold: {black.Gold} {Game.BlackPlayer.Gold} {white.Gold} {Game.WhitePlayer.Gold}");

                    if (color == PlayerColor.black)
                    {
                        black.Gold += stake;
                        Game.BlackPlayer.Gold += stake;
                    }
                    else
                    {
                        white.Gold += stake;
                        Game.WhitePlayer.Gold += stake;
                    }
                    Logger.LogInformation($"After transfer: {black.Gold} {Game.BlackPlayer.Gold} {white.Gold} {Game.WhitePlayer.Gold}");
                    Logger.LogInformation("Release Thread " + Thread.CurrentThread.ManagedThreadId);
                }

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
            Logger.LogInformation($"{winner} won Game {Game.Id} by resignition.");
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
                if (Game.GetHome(Player.Color.Black).Checkers.Count == 15)
                {
                    Game.PlayState = Game.State.Ended;
                    winner = PlayerColor.black;
                }
            }
            else
            {
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
                    throw new ApplicationException("An attempt to make an invalid move was made");
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

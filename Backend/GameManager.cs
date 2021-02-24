using Backend.Dto;
using Backend.Dto.Actions;
using Backend.Rules;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        public GameManager(ILogger<GameManager> logger)
        {
            Game = Game.Create();
            Created = DateTime.Now;
            Logger = logger;
        }

        private static List<GameManager> AllGames = new List<GameManager>();

        public WebSocket Client1 { get; set; }
        public WebSocket Client2 { get; set; }
        public Game Game { get; set; }

        public DateTime Created { get; private set; }
        private bool SearchingOpponent { get; set; }
        private ILogger<GameManager> Logger { get; }

        public static async Task Connect(WebSocket webSocket, HttpContext context, ILogger<GameManager> logger)
        {
            // find existing game to reconnect to.
            var cookies = context.Request.Cookies;
            var cookieKey = "backgammon-game-id";
            if (cookies.Any(c => (c.Key == cookieKey)))
            {
                var cookie = GameCookieDto.TryParse(cookies[cookieKey]);
                if (cookie != null)
                {
                    var game = AllGames.SingleOrDefault(g => g.Game.Id.ToString().Equals(cookie.id));
                    if (game != null)
                    {
                        logger.LogInformation($"Restoring game {cookie.id}");
                        await game.Restore(cookie.color, webSocket);
                        //This is end of connection
                        return;
                    }
                }
            }

            //todo: pair with someone equal ranking.
            var gameMananger = AllGames.OrderBy(g => g.Created)
                .FirstOrDefault(g => g.Client2 == null && g.SearchingOpponent);

            if (gameMananger == null)
            {
                gameMananger = new GameManager(logger);
                AllGames.Add(gameMananger);
                gameMananger.SearchingOpponent = true;
                logger.LogInformation($"Added a new game and waiting for opponent. Game id {gameMananger.Game.Id}");
                await gameMananger.ConnectAndListen(webSocket, Rules.Player.Color.Black);
                //This is end of connection
            }
            else
            {
                gameMananger.SearchingOpponent = false;
                logger.LogInformation($"Found a game and added a second player. Game id {gameMananger.Game.Id}");
                await gameMananger.ConnectAndListen(webSocket, Rules.Player.Color.White);
                //This is end of connection
            }
        }

        private void StartGame()
        {
            var gameDto = Game.ToDto();
            var action = new GameCreatedActionDto
            {
                game = gameDto
            };
            action.myColor = PlayerColor.black;
            _ = Send(Client1, action);
            action.myColor = PlayerColor.white;
            _ = Send(Client2, action);

            // todo: visa på clienten även när det blir samma 
            while (Game.PlayState == Game.State.FirstThrow)
            {
                Game.RollDice();
                var rollAction = new DicesRolledActionDto
                {
                    dices = Game.Roll.Select(d => d.ToDto()).ToArray(),
                    playerToMove = (PlayerColor)Game.CurrentPlayer,
                    validMoves = Game.ValidMoves.Select(m => m.ToDto()).ToArray()
                };
                _ = Send(Client1, rollAction);
                _ = Send(Client2, rollAction);
            }
        }

        private void SendNewRoll()
        {
            Game.RollDice();
            var rollAction = new DicesRolledActionDto
            {
                dices = Game.Roll.Select(d => d.ToDto()).ToArray(),
                playerToMove = (PlayerColor)Game.CurrentPlayer,
                validMoves = Game.ValidMoves.Select(m => m.ToDto()).ToArray()
            };
            _ = Send(Client1, rollAction);
            _ = Send(Client2, rollAction);
        }

        private async Task ConnectAndListen(WebSocket webSocket, Rules.Player.Color color)
        {
            if (color == Player.Color.Black)
            {
                Client1 = webSocket;
                await ListenOn(webSocket);
            }
            else
            {
                Client2 = webSocket;
                StartGame();
                await ListenOn(webSocket);
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
                catch (WebSocketException exc)
                {
                    Logger.LogInformation($"Cant receive data from socket, it was closed. ErrorCode: {exc.ErrorCode}");
                    return "";
                }
            }
            return sb.ToString();
        }

        internal async Task Restore(PlayerColor color, WebSocket socket)
        {
            var gameDto = Game.ToDto();
            var action = new GameRestoreActionDto
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

            await Send(socket, action);
            //Also send the state to the other client in case it has made moves.
            if (otherSocket.State == WebSocketState.Open)
            {
                action.color = color == PlayerColor.black ? PlayerColor.white : PlayerColor.black;
                await Send(otherSocket, action);
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
                    await DoAction(action.actionName, text, otherClient);
                }
            }
        }

        private async Task DoAction(ActionNames actionName, string text, WebSocket socket)
        {
            Logger.LogInformation($"Doing action: {actionName}");
            if (actionName == ActionNames.movesMade)
            {
                var action = (MovesMadeActionDto)JsonSerializer.Deserialize(text, typeof(MovesMadeActionDto));
                DoMoves(action);
                PlayerColor? winner = GetWinner();
                if (winner.HasValue)
                {
                    Logger.LogInformation($"Player {winner} won the game");
                    await SendWinner(winner.Value);
                    _ = CloseConnections();
                    AllGames.Remove(this);
                }
                else
                    SendNewRoll();
            }
            else if (actionName == ActionNames.opponentMove)
            {
                var action = (OpponentMoveActionDto)JsonSerializer.Deserialize(text, typeof(OpponentMoveActionDto));
                _ = Send(socket, action);
            }
            else if (actionName == ActionNames.undoMove)
            {   
                var action = (UndoActionDto)JsonSerializer.Deserialize(text, typeof(UndoActionDto));
                _ = Send(socket, action);
            }
            else if (actionName == ActionNames.connectionInfo)
            {
                var action = (ConnectionInfoActionDto)JsonSerializer.Deserialize(text, typeof(ConnectionInfoActionDto));
                _ = Send(socket, action);
            }
            else if (actionName == ActionNames.abortGame)
            {
                var action = (AbortGameActionDto)JsonSerializer.Deserialize(text, typeof(AbortGameActionDto));
                var winner = Client1 == socket ? PlayerColor.black : PlayerColor.white;
                _ = AbortGame(action.gameId, winner);                
            }
        }

        private async Task AbortGame(string gameId, PlayerColor winner)
        {
            if (Game.Id.ToString() != gameId)
            {
                Logger.LogWarning("A client is trying to abort a game that it is not connected to.");
                return;
            }

            Game.PlayState = Game.State.Ended;
            await SendWinner(winner);
            _ = CloseConnections();
            AllGames.Remove(this);
            Logger.LogInformation($"Game {gameId} removed.");
        }

        private async Task CloseConnections()
        {
            if (Client1 != null)
            {
                Logger.LogInformation("Closing client 1");
                await Client1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Game aborted by client", CancellationToken.None);
                Client1.Dispose();
            }
            if (Client2 != null)
            {
                Logger.LogInformation("Closing client 2");
                await Client2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Game aborted by client", CancellationToken.None);
                Client2.Dispose();
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
                    SendWinner(PlayerColor.white);
                }
            }

            return winner;
        }

        private void DoMoves(MovesMadeActionDto action)
        {
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
        
        private async Task SendWinner(PlayerColor color)
        {
            var game = Game.ToDto();
            game.winner = color;
            var gameEndedAction = new GameEndedActionDto
            {
                game = game
            };
            await Send(Client1, gameEndedAction);
            await Send(Client2, gameEndedAction);
        }

        private async Task Send<T>(WebSocket socket, T obj)
        {
            if (socket == null)
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

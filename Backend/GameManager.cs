using Backend.Dto;
using Backend.Dto.Actions;
using Backend.Rules;
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
        public GameManager(ILogger<Startup> logger)
        {
            Game = Game.Create();
            Created = DateTime.Now;
            Logger = logger;
        }

        public WebSocket Client1 { get; set; }
        public WebSocket Client2 { get; set; }
        public Game Game { get; set; }

        public DateTime Created { get; private set; }
        public bool SearchingOpponent { get; set; }
        public ILogger<Startup> Logger { get; }

        internal void StartGame()
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

        internal async Task ConnectAndListen(WebSocket webSocket, Rules.Player.Color color)
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

            if (color == PlayerColor.black)
                Client1 = socket;
            else
                Client2 = socket;

            await Send(socket, action);
            await ListenOn(socket);
        }

        private async Task ListenOn(WebSocket socket)
        {
            WebSocket otherClient = null;
            while (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted && socket.State != WebSocketState.CloseReceived)
            {
                var text = await ReceiveText(socket);
                if (text != null && text.Length > 0)
                {
                    Logger.LogInformation($"Received: {text}");
                    var action = (ActionDto)JsonSerializer.Deserialize(text, typeof(ActionDto));
                    otherClient = socket == Client1 ? Client2 : Client1;
                    DoAction(action.actionName, text, otherClient);
                }
            }

            if (otherClient != null && otherClient.State == WebSocketState.Open)
                _ = Send(otherClient, new ConnectionInfoActionDto { connection = new ConnectionDto { connected = false } });
        }

        private void DoAction(ActionNames actionName, string text, WebSocket otherClient)
        {
            Logger.LogInformation($"Doing action: {actionName}");
            if (actionName == ActionNames.movesMade)
            {
                var action = (MovesMadeActionDto)JsonSerializer.Deserialize(text, typeof(MovesMadeActionDto));
                DoMoves(action);
                //otherClient.Send(action);
                PlayerColor? winner = null;

                if (this.Game.CurrentPlayer == Player.Color.Black)
                {
                    this.Game.CurrentPlayer = Player.Color.White;
                    if (this.Game.GetHome(Player.Color.Black).Checkers.Count == 15)
                    {
                        this.Game.PlayState = Game.State.Ended;
                        winner = PlayerColor.black;
                    }
                }
                else
                {
                    this.Game.CurrentPlayer = Player.Color.Black;
                    if (this.Game.GetHome(Player.Color.White).Checkers.Count == 15)
                    {
                        this.Game.PlayState = Game.State.Ended;
                        winner = PlayerColor.white;
                        SendWinner(PlayerColor.white);
                    }
                }
                if (winner.HasValue)
                    SendWinner(winner.Value);
                else
                    SendNewRoll();
            }
            else if (actionName == ActionNames.opponentMove)
            {
                //No need to update the state because all moves will be sent
                var action = (OpponentMoveActionDto)JsonSerializer.Deserialize(text, typeof(OpponentMoveActionDto));
                _ = Send(otherClient, action);
            }
            else if (actionName == ActionNames.undoMove)
            {
                //No need to update the state because all moves will be sent
                var action = (UndoActionDto)JsonSerializer.Deserialize(text, typeof(UndoActionDto));
                _ = Send(otherClient, action);
            }
            else if (actionName == ActionNames.connectionInfo)
            {
                var action = (ConnectionInfoActionDto)JsonSerializer.Deserialize(text, typeof(ConnectionInfoActionDto));
                _ = Send(otherClient, action);
            }
        }

        private void DoMoves(MovesMadeActionDto action)
        {
            foreach (var moveDto in action.moves)
            {
                var color = (Player.Color)moveDto.color;
                var move = new Move
                {
                    Color = color,
                    From = Game.Points.Single(p => p.GetNumber(color) == moveDto.from),
                    To = Game.Points.Single(p => p.GetNumber(color) == moveDto.to),
                };
                // TODO: Check these moves are valid for safety.
                this.Game.MakeMove(move);
            }
        }

        private void SendWinner(PlayerColor color)
        {
            var game = this.Game.ToDto();
            game.winner = color;
            var gameEndedAction = new GameEndedActionDto
            {
                game = game
            };
            _ = Send(Client1, gameEndedAction);
            _ = Send(Client2, gameEndedAction);
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

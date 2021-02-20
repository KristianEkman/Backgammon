using Backend.Dto;
using Backend.Dto.Actions;
using Backend.Rules;
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
        public GameManager()
        {
            Game = Game.Create();
        }

        public WebSocket Client1 { get; set; }
        public WebSocket Client2 { get; set; }
        public Game Game { get; set; }

        private void StartGame()
        {
            var gameDto = Game.ToDto();
            var action = new GameCreatedActionDto
            {
                game = gameDto
            };
            action.myColor = PlayerColor.black;
            Client1.Send(action);
            action.myColor = PlayerColor.white;
            Client2.Send(action);

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
                Client1.Send(rollAction);
                Client2.Send(rollAction);
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
            Client1.Send(rollAction);
            Client2.Send(rollAction);
        }

        internal async Task ConnectSocket(WebSocket webSocket)
        {
            if (Client1 == null)
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
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var text = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                sb.Append(text);
            }
            return sb.ToString();
        }

        private async Task ListenOn(WebSocket socket)
        {
            var otherClient = socket == Client1 ? Client2 : Client1;
            try
            {
                while (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted)
                {
                    var text = await ReceiveText(socket);
                    var action = (ActionDto)JsonSerializer.Deserialize(text, typeof(ActionDto));
                    DoAction(action.actionName, text, otherClient);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (otherClient != null)
                {
                    otherClient.Send(new ConnectionInfoActionDto
                    {
                        connection = new ConnectionDto{connected = false}
                    }); ;

                }
            }
        }

        private void DoAction(ActionNames actionName, string text, WebSocket otherClient)
        {
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
                otherClient.Send(action);
            }
            else if (actionName == ActionNames.undoMove)
            {
                //No need to update the state because all moves will be sent
                var action = (UndoActionDto)JsonSerializer.Deserialize(text, typeof(UndoActionDto));
                otherClient.Send(action);
            }
            else if (actionName == ActionNames.connectionInfo)
            {
                var action = (ConnectionInfoActionDto)JsonSerializer.Deserialize(text, typeof(ConnectionInfoActionDto));
                otherClient.Send(action);
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
            Client1.Send(gameEndedAction);
            Client2.Send(gameEndedAction);
        }
    }

    public static class SocketExtentions
    {
        public static async Task Send<T>(this WebSocket socket, T obj)
        {
            if (socket == null)
            {
                Console.WriteLine("Cannot send to socket, it was lost.");
                return;
            }
            var json = JsonSerializer.Serialize<object>(obj);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

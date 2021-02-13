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
            action.game.myColor = PlayerColor.black;
            Client1.Send(action);
            action.game.myColor = PlayerColor.white;
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

        private async Task ListenOn(WebSocket socket)
        {
            var buffer = new byte[1024];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var text = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                var action = (ActionDto)JsonSerializer.Deserialize(text, typeof(ActionDto));
                var otherClient = socket == Client1 ? Client2 : Client1;                
                DoAction(action.actionName, text, otherClient);
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
        }

        private void DoAction(ActionNames actionName, string text, WebSocket otherClient)
        {
            if (actionName == ActionNames.movesMade)
            {
                var action = (MovesMadeActionDto)JsonSerializer.Deserialize(text, typeof(MovesMadeActionDto));
                DoMoves(action);
                otherClient.Send(action);
                SendNewRoll();
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
            if (this.Game.CurrentPlayer == Player.Color.Black)
                this.Game.CurrentPlayer = Player.Color.White;
            else
                this.Game.CurrentPlayer = Player.Color.Black;
        }
    }

    public static class SocketExtentions
    {
        public static async Task Send<T>(this WebSocket socket, T obj)
        {
            var json = JsonSerializer.Serialize<object>(obj);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

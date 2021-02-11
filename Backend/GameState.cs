using Backend.Dto;
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
    public class GameState
    {
        public GameState()
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
            Client1.Send(action);
            Client2.Send(action);
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

                //JsonSerializer.Deserialize(text, theType);
                // todo, move checkers or other things
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            }
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

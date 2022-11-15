using Backend.Dto;
using Backend.Dto.chat;
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
    public class ChatService
    {
        internal static List<ChatClient> AllClients = new List<ChatClient>();

        internal static async Task Connect(WebSocket webSocket, HttpContext context, ILogger<GameManager> logger, string userId)
        {
            if (Maintenance())
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Backgammon temporary closed for maintenance.", CancellationToken.None);
                return;
            }

            var dbUser = Db.BgDbContext.GetDbUser(userId);

            var client = AllClients.FirstOrDefault(cl => cl.UserId == dbUser.Id);
            if (client == null)
            {
                client = new ChatClient(dbUser.Id, webSocket);
                //manager.Ended += Game_Ended;
                client.MessageReceived += Client_MessageReceived;

                AllClients.Add(client);
                logger.LogInformation($"Added a new chat client");
                
                // entering socket loop
                await client.ListenOn();
                //This is the end of the connection
            }

            AllClients.Remove(client);
        }

        private static void Client_MessageReceived(ChatClient sender, ChatDto dto)
        {
            foreach (var client in AllClients)
            {
                _ = client.Send(dto);
            }
        }

        private static bool Maintenance()
        {
            using (var db = new Db.BgDbContext())
            {
                var m = db.Maintenance.OrderByDescending(m => m.Time).FirstOrDefault();
                return m != null && m.On;
            }
        }


        private static async Task SendConnectionLost(PlayerColor color, GameManager manager)
        {
            var socket = manager.Client1;
            if (color == PlayerColor.white)
                socket = manager.Client2;
            if (socket != null && socket.State == WebSocketState.Open)
            {
                var action = new Dto.Actions.ConnectionInfoActionDto
                {
                    connection = new ConnectionDto
                    {
                        connected = false
                    }
                };
                await manager.Send(socket, action);
            }
        }

        private static bool MyColor(GameManager manager, Db.User dbUser, PlayerColor color)
        {
            //prevents someone with same game id, get someone elses side in the game.
            var player = manager.Game.BlackPlayer;
            if (color == PlayerColor.white)
                player = manager.Game.WhitePlayer;

            return dbUser != null && dbUser.Id == player.Id;
        }

        private static void Game_Ended(object sender, EventArgs e)
        {
            AllClients.Remove(sender as ChatClient);
        }
    }
}

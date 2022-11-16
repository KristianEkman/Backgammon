using Backend.Db;
using Backend.Dto;
using Backend.Dto.chat;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
                client = new ChatClient(dbUser.Id, dbUser.Name, webSocket, logger);
                client.MessageReceived += Client_MessageReceived;

                AllClients.Add(client);
                SendJoined();

                logger.LogInformation($"Added a new chat client");

                // entering socket loop
                await client.ListenOn();

                //This is the end of the connection
                logger.LogInformation("Chat client disconnected");
            }
            else
            {
                logger.LogInformation("Reuse previous client");
            }

        }

        private static void SendJoined()
        {
            var users = AllClients.Select(c => c.UserName).ToArray();
            foreach (var clnt in AllClients)
            {
                _ = clnt.Send(
                    new JoinedChatDto {
                        type = nameof(JoinedChatDto),
                        users = users
                    });
            }
        }
        
        private static void Client_MessageReceived(ChatClient sender, ChatDto dto)
        {
            foreach (var clnt in AllClients)
                _ = clnt.Send(dto);

            if (dto.type == nameof(LeftChatDto))
            {
                AllClients.Remove(sender);
                sender.Disconnect();
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
    }
}

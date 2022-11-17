using Backend.Dto;
using Backend.Dto.chat;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class ChatClient
    {
        public Guid UserId { get; }
        public string UserName { get; }
        public WebSocket Socket { get; }
        ILogger<GameManager> Logger { get; }

        public ChatClient(Guid userId, string userName, WebSocket socket, ILogger<GameManager> logger)
        {
            this.UserId = userId;
            this.Socket = socket;
            this.UserName = userName;
            Logger = logger;
        }

        internal event DtoReceivedEvent MessageReceived;

        internal async Task ListenOn()
        {
            while (Socket.State != WebSocketState.Closed &&
                Socket.State != WebSocketState.Aborted &&
                Socket.State != WebSocketState.CloseReceived)
            {
                var text = await ReceiveText();
                if (text != null && text.Length > 0)
                {
                    Logger.LogInformation($"Received: {text}");
                    try
                    {
                        var dto = (ChatDto)JsonSerializer.Deserialize(text, typeof(ChatDto));
                        if (dto.type == nameof(ChatMessageDto))
                            MessageReceived?.Invoke(this, (ChatMessageDto)JsonSerializer.Deserialize(text, typeof(ChatMessageDto)));
                        else if (dto.type == nameof(ChatUsersDto))
                            MessageReceived?.Invoke(this, (ChatUsersDto)JsonSerializer.Deserialize(text, typeof(ChatUsersDto)));
                        else if (dto.type == nameof(LeftChatDto))
                            MessageReceived?.Invoke(this, (LeftChatDto)JsonSerializer.Deserialize(text, typeof(LeftChatDto)));
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, $"Failed to parse Action text {text}");
                    }
                }
            }
        }

        private async Task<string> ReceiveText()
        {
            var buffer = new byte[512];
            var sb = new StringBuilder();
            WebSocketReceiveResult result = null;
            while (result == null || (!result.EndOfMessage && !result.CloseStatus.HasValue))
            {
                try
                {
                    result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var text = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                    sb.Append(text);
                }
                catch (Exception exc)
                {
                    Logger.LogError($"Can't receive data from socket. Error: {exc.ToString()}");
                    return "";
                }
            }
            return sb.ToString();
        }

        internal async Task Send<T>(T obj)
        {
            if (Socket == null || Socket.State != WebSocketState.Open)
            {
                Logger.LogInformation("Cannot send to socket, connection was lost.");
                return;
            }
            var json = JsonSerializer.Serialize<object>(obj);
            // Logger.LogInformation($"Sending to client ${json}");
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            try
            {
                await Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception exc)
            {
                    Logger.LogError($"Failed to send socket data. Exception: {exc}");
            }
        }

        internal void Disconnect()
        {
            Logger.LogInformation($"Closing chat socket for ${UserName}");
            Socket.Abort();
        }
    }

    public delegate void DtoReceivedEvent(ChatClient sender, ChatDto dto);
}

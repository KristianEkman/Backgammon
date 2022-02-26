using Backend.Dto;
using Backend.Rules;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class GamesService
    {
        internal static List<GameManager> AllGames = new List<GameManager>();

        internal static async Task Connect(WebSocket webSocket, HttpContext context, ILogger<GameManager> logger, string userId, string gameId, bool playAi, bool forGold)
        {
            if (Maintenance())
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Backgammon temporary closed for maintenance.", CancellationToken.None);
                return;
            }

            var dbUser = Db.BgDbContext.GetDbUser(userId);

            if (await TryReConnect(webSocket, context, logger, dbUser))
            {
                // Game disconnected here
                return;
            }

            if (!string.IsNullOrWhiteSpace(gameId))
            {
                await ConnectInvite(webSocket, dbUser, gameId);
                // Game disconnected here.
                return;
            }

            //todo: pair with someone equal ranking?
            
            // Search any game, oldest first.
            var managers = AllGames.OrderByDescending(g => g.Created)
                .Where(g => (g.Client2 == null|| g.Client1 == null) && g.SearchingOpponent);

            if (GameAlreadyStarted(managers, userId))
            {
                string warning = $"The user {userId} has already started a game";
                logger.LogWarning(warning);
                throw new ApplicationException(warning);
            }
                       
            var isGuest = dbUser.Id == Guid.Empty;
            // filter out games having a logged in player            
            if (isGuest)
                managers = managers.Where(m => !(
                    m.Game.BlackPlayer.Id != Guid.Empty) ||
                    m.Game.WhitePlayer.Id != Guid.Empty
                ).ToArray();

            var manager = managers.FirstOrDefault();
            if (manager == null || playAi)
            {
                manager = new GameManager(logger, forGold);
                manager.Ended += Game_Ended;
                manager.SearchingOpponent = !playAi;
                AllGames.Add(manager);
                logger.LogInformation($"Added a new game and waiting for opponent. Game id {manager.Game.Id}");
                // entering socket loop
                await manager.ConnectAndListen(webSocket, Player.Color.Black, dbUser, playAi);
                await SendConnectionLost(PlayerColor.white, manager);
                //This is the end of the connection
            }
            else
            {
                manager.SearchingOpponent = false;
                logger.LogInformation($"Found a game and added a second player. Game id {manager.Game.Id}");
                var color = manager.Client1 == null ? Player.Color.Black : Player.Color.White;
                // entering socket loop
                await manager.ConnectAndListen(webSocket, color, dbUser, false);
                logger.LogInformation($"{color} player disconnected.");
                await SendConnectionLost(PlayerColor.black, manager);
                //This is the end of the connection
            }
            RemoveDissconnected(manager);
        }

        private static bool GameAlreadyStarted(IEnumerable<GameManager> managers, string userId)
        {
            foreach (var m in managers)
            {
                // Guest vs guest must be allowed. When guest games are enabled.
                if (m.Game.BlackPlayer.Id.ToString() == userId || m.Game.WhitePlayer.Id.ToString() == userId && userId != Guid.Empty.ToString())
                    return true;
            }
            return false;
        }

        internal static void SaveState()
        {
            var state = JsonSerializer.Serialize<List<GameManager>>(AllGames);
            System.IO.File.WriteAllText("SavedGames.json", state);
        }

        internal static void RestoreState(ILogger<GameManager> logger)
        {
            var state = System.IO.File.ReadAllBytes("SavedGames.json");
            AllGames = JsonSerializer.Deserialize<List<GameManager>>(state);
            AllGames = AllGames.Where(g => g.Created > DateTime.Now.Date).ToList();
            foreach (var game in AllGames)
                game.Logger = logger;
        }

        internal static Guid CreateInvite(ILogger<GameManager> logger, string userId)
        {
            var existing = AllGames.Where(
                g => g.Inviter == userId).ToArray();

            for (int i = existing.Length - 1; i >= 0; i--)
                AllGames.Remove(existing[i]);

            var manager = new GameManager(logger, true);
            manager.Ended += Game_Ended;
            manager.Inviter = userId;
            manager.SearchingOpponent = false;
            AllGames.Add(manager);
            return manager.Game.Id;
        }

        private static bool Maintenance()
        {
            using (var db = new Db.BgDbContext())
            {
                var m = db.Maintenance.OrderByDescending(m => m.Time).FirstOrDefault();
                return m != null && m.On;
            }
        }

        private static async Task<bool> TryReConnect(WebSocket webSocket, HttpContext context, ILogger<GameManager> logger, Db.User dbUser)
        {
            var cookies = context.Request.Cookies;
            var cookieKey = "backgammon-game-id";
            // Find existing game to reconnect to.
            if (cookies.Any(c => (c.Key == cookieKey)))
            {
                var cookie = GameCookieDto.TryParse(cookies[cookieKey]);
                var color = cookie.color;

                if (cookie != null)
                {
                    var gameManager = AllGames
                        .SingleOrDefault(g =>
                            g.Game.Id.ToString().Equals(cookie.id) &&
                            g.Game.PlayState != Game.State.Ended
                         );

                    if (gameManager != null && MyColor(gameManager, dbUser, color))
                    {
                        gameManager.Engine = new Ai.Engine(gameManager.Game);
                        logger.LogInformation($"Restoring game {cookie.id} for {color}");
                        // entering socket loop
                        await gameManager.Restore(color, webSocket);
                        var otherColor = color == PlayerColor.black ?
                            PlayerColor.white : PlayerColor.black;
                        await SendConnectionLost(otherColor, gameManager);
                        // socket loop exited
                        RemoveDissconnected(gameManager);
                        return true;
                    }
                }
            }
            return false;
        }

        private static async Task ConnectInvite(WebSocket webSocket, Db.User dbUser, string gameInviteId)
        {
            var manager = AllGames.SingleOrDefault(g => g.Game.Id.ToString() == gameInviteId && (g.Client1 == null || g.Client2 == null));
            if (manager == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invite link has expired", CancellationToken.None);
                return;
            }

            var color = Player.Color.Black;
            if (manager.Client1 != null)
                color = Player.Color.White;

            await manager.ConnectAndListen(webSocket, color, dbUser, false);
            RemoveDissconnected(manager);
            await SendConnectionLost(PlayerColor.white, manager);
        }

        private static void RemoveDissconnected(GameManager manager)
        {
            if ((manager.Client1 == null || manager.Client1.State != WebSocketState.Open) &&
                (manager.Client2 == null || manager.Client2.State != WebSocketState.Open))
            {
                AllGames.Remove(manager);
                manager.Logger.LogInformation($"Removing game {manager.Game.Id} which is not used.");
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
            AllGames.Remove(sender as GameManager);
        }        
    }
}

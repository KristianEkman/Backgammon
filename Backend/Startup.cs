using Backend.Dto;
using Backend.Dto.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class Startup
    {
        // is this realy persistent
        static List<GameManager> AllGames = new List<GameManager>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation($"Configure startup.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseWebSockets();

            app.UseDefaultFiles();


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        logger.LogInformation($"New web socket request.");

                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

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
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();

                    //if (context.Request.Path.ToString().Contains("index.html") || SinglePageAppRequestCheck(context))
                    //{
                    // add config cookie
                    //var webConfig = new WebConfigModel();
                    //Configuration.Bind(webConfig);
                    //var jsonSettings = new JsonSerializerSettings()
                    //{
                    //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                    //};
                    //context.Response.Cookies.Append("ldmeConfig", JsonConvert.SerializeObject(webConfig, Formatting.None, jsonSettings));
                    //}

                    // This enables angular routing to function side on the same app as the web socket.
                    // If there's no available file and the request doesn't contain an extension, we're probably trying to access a page.
                    // Rewrite request to use app root
                    if (SinglePageAppRequestCheck(context))
                    {
                        context.Request.Path = "/index.html";
                        await next();
                    }
                }
            });

            app.UseStaticFiles();
        }

        private static bool SinglePageAppRequestCheck(HttpContext context)
        {
            return context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value);
        }


        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var text = $"Från sörver: {DateTime.Now.ToString("sss.ffffff") } ";
                var list = new List<byte>(System.Text.Encoding.UTF8.GetBytes(text));
                list.AddRange(buffer.Take(result.Count));
                buffer = list.ToArray();
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}

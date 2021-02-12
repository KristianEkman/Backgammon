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
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class Startup
    {
        // is this realy persistent
        static List<GameManager> gameQueue = new List<GameManager>();

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        //await Echo(webSocket);

                        //todo: pair with someone equal ranking.

                        // TODO: Remove games with lost connections.
                        var gameState = gameQueue.FirstOrDefault(g => g.Client2 == null);
                        if (gameState == null)
                        {
                            gameState = new GameManager();
                            gameQueue.Add(gameState);
                            await gameState.ConnectSocket(webSocket);
                        }
                        else
                        {
                            gameQueue.Remove(gameState);
                            await gameState.ConnectSocket(webSocket);
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
                }
            });
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

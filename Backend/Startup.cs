using Backend.Dto;
using Backend.Dto.Actions;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<GameManager> logger, IHostApplicationLifetime applicationLifetime)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backgammon Backend v1"));
            }

            TryRestoreState(logger);

            applicationLifetime.ApplicationStopping.Register(() => { OnShutdown(logger); });

            app.UseCors(options => options.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin()
            );

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseWebSockets();
            app.UseDefaultFiles();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws/game")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        logger.LogInformation($"New web socket request.");

                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var userId = context.Request.Query.FirstOrDefault(q => q.Key == "userId").Value;
                        var gameId = context.Request.Query.FirstOrDefault(q => q.Key == "gameId").Value;
                        try
                        {
                            await GameManager.Connect(webSocket, context, logger, userId, gameId);
                        }
                        catch (Exception exc)
                        {
                            logger.LogError(exc.ToString());
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

                    // This enables angular routing to function on the same app as the web socket.
                    // If there's no available file and the request doesn't contain an extension, we're probably trying to access a page.
                    // Rewrite request to use app root
                    if (SinglePageAppRequestCheck(context))
                    {
                        context.Request.Path = "/index.html";
                        await next();
                    }
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
        }

        private void TryRestoreState(ILogger<GameManager> logger)
        {
            try
            {
                GameManager.RestoreState(logger);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }

        private static bool SinglePageAppRequestCheck(HttpContext context)
        {
            return context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value);
        }

        private void OnShutdown(ILogger<GameManager> logger)
        {
            try
            {
                GameManager.SaveState();
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }
    }
}

using Backend.Controllers;
using Backend.Dto;
using Backend.Dto.Actions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            
            Db.BgDbContext.ConnectionsString = configuration.GetSection("ConnectionStrings");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllers(
                options => options.Filters.Add(new ServiceInterceptor()));

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

            // TryRestoreState(logger);

            applicationLifetime.ApplicationStopping.Register(() => { OnShutdown(logger); });

            app.UseCors(options => options.WithOrigins("https://localhost:4200")
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
                        var playAi = context.Request.Query.FirstOrDefault(q => q.Key == "playAi").Value == "true";
                        var forGold = context.Request.Query.FirstOrDefault(q => q.Key == "forGold").Value == "true";
                        try
                        {
                            await GamesService.Connect(webSocket, context, logger, userId, gameId, playAi, forGold);
                        }
                        catch (Exception exc)
                        {
                            logger.LogError(exc, "Failed to connect to GameService");
                            await context.Response.WriteAsync(exc.Message, CancellationToken.None);
                            context.Response.StatusCode = 400;
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

        private static void TryRestoreState(ILogger<GameManager> logger)
        {
            try
            {
                GamesService.RestoreState(logger);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to restore state.");
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
                GamesService.SaveState();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to save State");
            }
        }
    }

    public class ServiceInterceptor : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //AssertOrigin(context.HttpContext.Request);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after 
        }

        public static void AssertOrigin(HttpRequest request)
        {
            var origin = request.Headers["Referer"].ToString();
            // Need to be allowed for email unsubscibe feature.
            if (string.IsNullOrEmpty(origin))
                return;

            var allowed = new List<string>();
            allowed.Add("https://backgammon.azurewebsites.net");            
            allowed.Add("https://backgammon-slot1.azurewebsites.net");
#if DEBUG
            allowed.Add("http://localhost");
#endif
            if (!allowed.Any(x => origin.StartsWith(x)))
                throw new UnauthorizedAccessException("Origin not allowed");
        }
    }
}

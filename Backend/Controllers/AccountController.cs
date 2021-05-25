using Backend.Db;
using Backend.Dto;
using Backend.Dto.message;
using Backend.Dto.rest;
using Google.Apis.Auth;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [ApiController]
    public class AccountController : AuthorizedController
    {
        private readonly ILogger<AccountController> logger;

        public AccountController(ILogger<AccountController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        [Route("/api/account/signin")]
        public async Task<UserDto> Post(UserDto userDto)
        {
            // todo: 
            //var settings = new GoogleJsonWebSignature.ValidationSettings
            //{
            //	HostedDomain = "http://localhost:4200"				
            //};

            bool valid = false;
            try
            {
                if (userDto.socialProvider == "GOOGLE")
                {
                    var validPayload = await GoogleJsonWebSignature.ValidateAsync(Request.Headers["Authorization"]);
                    // todo: more validation?
                    valid = validPayload != null;
                }
                if (userDto.socialProvider == "FACEBOOK")
                {
                    valid = await ValidateFacebookJwt(Request.Headers["Authorization"]);
                }
            }
            catch (Exception exc)
            {
                logger.LogError(exc.ToString());
            }

            if (!valid)
                return null;

            return GetOrCreateLogin(userDto);

        }

        private UserDto GetOrCreateLogin(UserDto userDto)
        {
            using (var db = new BgDbContext())
            {
                var dbUser = db.Users.SingleOrDefault(user =>
                    user.SocialProvider.Equals(userDto.socialProvider) &&
                    user.ProviderId.Equals(userDto.socialProviderId)
                    );

                // todo: is this safe or should email be checked instead or also?
                if (dbUser != null)
                {
                    return dbUser.ToDto();
                }
                else
                {
                    dbUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = userDto.email,
                        Name = userDto.name,
                        PhotoUrl = userDto.photoUrl,
                        ProviderId = userDto.socialProviderId,
                        SocialProvider = userDto.socialProvider,
                        Elo = 1200,
                        Registered = DateTime.Now,
                        Admin = false,
                        EmailNotifications = true,
                        EmailUnsubscribeId = Guid.NewGuid(),
                        Theme = "dark",
                        PreferredLanguage = "en",
                        Gold = 200,
                        LastFreeGold = DateTime.Now,                        
                    };
                    db.Users.Add(dbUser);

                    // Give new users a prompt message to share the site.
                    var admin = db.Users.First(u => u.Admin);
                    dbUser.ReceivedMessages.Add(new Message
                    {
                        Text = "",
                        Type = Dto.message.MessageType.SharePrompt,
                        Sender = admin,
                        Sent = DateTime.Now
                    });

                    db.SaveChanges();

                    // The id will not be set until the save is successfull.
                    userDto.id = dbUser.Id.ToString();
                    userDto.createdNew = true;
                }
                return userDto;
            }
        }

        [HttpPost]
        [Route("/api/account/saveuser")]
        public void SaveUser(UserDto userDto)
        {
            var usId = GetUserId();
            using (var db = new Db.BgDbContext())
            {
                var dbUser = db.Users.Single(u => u.Id.ToString() == usId);
                dbUser.Name = userDto.name;
                dbUser.PreferredLanguage = userDto.preferredLanguage;
                dbUser.Theme = userDto.theme;
                dbUser.EmailNotifications = userDto.emailNotification;
                db.SaveChanges();
            }
        }

        [HttpPost]
        [Route("/api/account/delete")]
        public void DeleteUserAccount(UserDto userDto)
        {
            var usId = GetUserId();
            // some kind of safety check.
            if (userDto.id != usId)
                throw new ApplicationException("User id missmatch"); // The id in the request header should always be the same as sent by client.

            using (var db = new Db.BgDbContext())
            {
                var dbUser = db.Users.Single(u => u.Id.ToString() == usId);
                // maybe delete all records in player table, (has to be modeled out)                
                dbUser.Name = "deleted";
                dbUser.Elo = 0;
                dbUser.SocialProvider = "";
                dbUser.Email = "";
                dbUser.GameCount = 0;
                dbUser.PhotoUrl = "";
                dbUser.ProviderId = "";
                dbUser.PreferredLanguage = "";
                dbUser.Admin = false;
                db.SaveChanges();
            }
        }

        [HttpPost]
        [Route("/api/account/requestgold")]
        public GoldGiftDto RequestGold()
        {
            var usId = GetUserId();
            using (var db = new Db.BgDbContext())
            {
                var dbUser = db.Users.Single(u => u.Id.ToString() == usId);
                if (dbUser.Gold < GoldGiftDto.Gift && DateTime.Now > dbUser.LastFreeGold.AddDays(1))
                {
                    dbUser.Gold = GoldGiftDto.Gift;
                    dbUser.LastFreeGold = DateTime.Now;
                    db.SaveChanges();
                    return new GoldGiftDto
                    {
                        Gold = GoldGiftDto.Gift,
                        NextGiftAllowed = DateTime.Now.AddDays(1)
                    };
                }

                return new GoldGiftDto
                {
                    Gold = 0,
                    NextGiftAllowed = dbUser.LastFreeGold.AddDays(1)
                };
            }
        }

        private async Task<bool> ValidateFacebookJwt(string token)
        {
            var appToken = Secrets.FbAppToken();
            var sReq = $"https://graph.facebook.com/debug_token?input_token={token}&access_token={appToken}";
            var request = new HttpRequestMessage(HttpMethod.Get, sReq);
            var isValid = false;

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.SendAsync(request);
                    var responseString = "";
                    if (response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                        isValid = responseString.Contains(",\"is_valid\":true,");
                        if (isValid)
                            logger.LogInformation("Facebook auth token found valid.");
                        else
                            logger.LogWarning($"A facebook auth token found invalid. {responseString}");
                    }
                    else
                    {
                        logger.LogError($"Facebook login jwt was not valid. {responseString}");
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    logger.LogError(exc.ToString());
                }
            }

            return isValid;
        }
    }
}

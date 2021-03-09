using Backend.Db;
using Backend.Dto;
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
    public class SigninController : ControllerBase
    {
        private readonly ILogger<SigninController> logger;

        public SigninController(ILogger<SigninController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        [Route("/api/signin")]
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
                    userDto.id = dbUser.Id.ToString();
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
                        SocialProvider = userDto.socialProvider
                    };
                    db.Users.Add(dbUser);
                    db.SaveChanges();

                    // The id will not be set until the save is successfull.
                    userDto.id = dbUser.Id.ToString();
                }
                return userDto;
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

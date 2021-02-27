using Backend.Db;
using Backend.Dto;
using Google.Apis.Auth;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [ApiController]
    public class SigninController : ControllerBase
    {

        [HttpPost]
        [Route("/api/signin")]
        public async Task<UserDto> Post(UserDto userDto)
        {
            // todo: 
            //var settings = new GoogleJsonWebSignature.ValidationSettings
            //{
            //	HostedDomain = "http://localhost:4200"				
            //};

            bool valid;
            try
            {
				var validPayload = await GoogleJsonWebSignature.ValidateAsync(Request.Headers["Authorization"]);
				// todo: more validation?
                valid = validPayload != null;				
			}
			catch (Exception)
            {
				//todo: log error
				return null;
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
    }
}

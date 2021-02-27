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
    //[Authorize]
    public class SigninController : ControllerBase
    {

        [HttpGet]
        [Route("/api/signin")]
        public async Task<bool> Get()
        {
            // todo: 
			//var settings = new GoogleJsonWebSignature.ValidationSettings
			//{
			//	HostedDomain = "http://localhost:4200"				
			//};

            try
            {
				var validPayload = await GoogleJsonWebSignature.ValidateAsync(Request.Headers["Authorization"]);
				bool valid = validPayload != null;
				return valid;
			}
			catch (Exception)
            {
				//todo: loggong
				return false;
            }
			
        }


	}
}

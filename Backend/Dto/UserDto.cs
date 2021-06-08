using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto
{
    public class UserDto
    {

        public string id { get; set; }        
        public string name { get; set; }
        public string email { get; set; }
        public string photoUrl { get; set; }
        public bool showPhoto { get; set; }
        public string socialProvider { get; set; }
        public string socialProviderId { get; set; }
        public bool createdNew { get; set; }
        public bool isAdmin { get; set; }
        public string preferredLanguage { get; set; }
        public string theme { get; set; }
        public bool emailNotification { get; set; }
        public int gold { get; set; }
        public int lastFreeGold { get; set; }
        public int elo { get; set; }


    }
}

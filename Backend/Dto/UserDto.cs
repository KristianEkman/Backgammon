﻿using System;
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
        public string socialProvider { get; set; }
        public string socialProviderId { get; set; }
    }
}
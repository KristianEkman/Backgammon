using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.account
{
    public class LocalLoginDto
    {
        public string email { get; set; }
        public int passHash { get; set; }
    }
}

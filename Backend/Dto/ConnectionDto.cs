using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto
{
    public class ConnectionDto
    {
        public bool connected { get; set; }
        public int pingMs { get; set; }
    }
}

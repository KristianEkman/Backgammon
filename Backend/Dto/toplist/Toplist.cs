using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.toplist
{
    public class Toplist
    {
        public ToplistResult[] results { get; set; }
        public ToplistResult you { get; set; }
    }
}

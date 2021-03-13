using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.toplist
{
    public class ToplistResult
    {
        public int place { get; set; }
        public string name { get; set; }
        public int elo { get; set; }
        public bool you { get; set; }
    }
}

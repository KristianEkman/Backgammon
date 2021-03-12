using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class Maintenance
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public bool On { get; set; }

    }
}

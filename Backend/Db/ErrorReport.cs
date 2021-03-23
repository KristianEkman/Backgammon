using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class ErrorReport
    {
        public int Id { get; set; }
        public string Error { get; set; }
        public string Reproduce { get; set; }
        public User Reporter { get; set; }
    }
}

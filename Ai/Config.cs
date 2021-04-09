using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai
{
    public class Config
    {
        public bool PropabilityScore { get; set; } = false;
        public double PropabilityFactor { get; set; } = 0.25;
        public bool HitableBad { get; set; } = false;
    }
}

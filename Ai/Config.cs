using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai
{
    public class Config
    {
        /// <summary>
        /// Hitables checkers below this threshold does not reduce score.
        /// </summary>
        public int HitableThreshold { get; set; } = 4;

        internal Config Clone()
        {
            return new Config
            {
                BlockedPointScore = BlockedPointScore,
                ConnectedBlocksFactor = ConnectedBlocksFactor,
                HitableFactor = HitableFactor,
                HitableThreshold = HitableThreshold
            };
        }

        /// <summary>
        /// The point divided by this factor reduces score for hitable checkers.
        /// </summary>
        public double HitableFactor { get; set; } = 1.14d;

        // Score received for one point blocked.
        public double BlockedPointScore { get; set; } = 1.5d;

        // Score from number of consecutive blocks raised to this value.
        public double ConnectedBlocksFactor { get; set; } = 1.5;

        public override string ToString()
        {
            return $"HF: {HitableFactor.ToString("0.##")} HT: {HitableThreshold} CB: {ConnectedBlocksFactor.ToString("0.##")} BP: {BlockedPointScore.ToString("0.##")}";   
        }

    }
}

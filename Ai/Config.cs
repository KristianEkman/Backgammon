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
        /// Bloats are hitables checkers. Below this threshold does not reduce score.
        /// </summary>
        public int BloatsThreshold { get; set; } = 6;

        internal Config Clone()
        {
            return new Config
            {
                BlockedPointScore = BlockedPointScore,
                ConnectedBlocksFactor = ConnectedBlocksFactor,
                BloatsFactor = BloatsFactor,
                BloatsThreshold = BloatsThreshold,
                RunOrBlockFactor = RunOrBlockFactor
            };
        }

        /// <summary>
        /// The point divided by this factor reduces score for bloats.
        /// </summary>
        public double BloatsFactor { get; set; } = 1.65978574;

        // Score received for one point blocked.
        public double BlockedPointScore { get; set; } = 2.158432;

        // Score from number of consecutive blocks raised to this value.
        public double ConnectedBlocksFactor { get; set; } = 1.54848384;

        /// <summary>
        /// When all checkers have passed each other, the leading side gets a score bonus
        /// this factor multiplied by the lead.
        /// </summary>
        public double RunOrBlockFactor { get; set; } = 1.261879448;
        public bool ProbablityScore { get; set; } = false;

        public override string ToString()
        {
            return $"BF: {BloatsFactor.ToString("0.##")} BT: {BloatsThreshold} CB: {ConnectedBlocksFactor.ToString("0.##")} BP: {BlockedPointScore.ToString("0.##")} RB: {RunOrBlockFactor.ToString("0.##")}";   
        }
    }
}

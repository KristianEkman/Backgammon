using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai
{
    public class Config
    {
        

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
        /// Bloats are hitables checkers. Below this threshold does not reduce score.
        /// </summary>
        public int BloatsThreshold { get; set; }

        /// <summary>
        /// The point divided by this factor reduces score for bloats.
        /// </summary>
        public double BloatsFactor { get; set; }

        /// <summary>
        /// Score received for one point blocked.
        /// </summary>
        public double BlockedPointScore { get; set; }

        /// <summary>
        /// Score from number of consecutive blocks raised to this value.
        /// </summary>
        public double ConnectedBlocksFactor { get; set; }

        /// <summary>
        /// When all checkers have passed each other, the leading side gets a score bonus
        /// this factor multiplied by the lead.
        /// </summary>
        public double RunOrBlockFactor { get; set; }
        public bool ProbablityScore { get; set; } = false;

        public override string ToString()
        {
            return $"BF: {BloatsFactor.ToString("0.##")} BT: {BloatsThreshold} CB: {ConnectedBlocksFactor.ToString("0.##")} BP: {BlockedPointScore.ToString("0.##")} RB: {RunOrBlockFactor.ToString("0.##")}";
        }

        public static Config Zero()
        {
            return new Config
            {
                BloatsFactor = 0,
                BloatsThreshold = 0,
                BlockedPointScore = 0,
                ConnectedBlocksFactor = 0,
                ProbablityScore = false,
                RunOrBlockFactor = 0
            };
        }

        public static Config Trained()
        {
            return new Config
            {
                BloatsFactor = 1.65978574,
                BloatsThreshold = 6,
                BlockedPointScore = 2.158432,
                ConnectedBlocksFactor = 1.54848384,
                ProbablityScore = false,
                RunOrBlockFactor = 1.261879448
            };
        }

        public static Config NoDoubles41Epochs()
        {
            return new Config
            {
                BloatsFactor = 1.747286362,
                BloatsThreshold = 14,
                BlockedPointScore = 1.145912,
                ConnectedBlocksFactor = 2.019573916,
                ProbablityScore = false,
                RunOrBlockFactor = 0.838315223
            };
        }

        public static Config NoDoubles20Epochs()
        {
            return new Config
            {
                BloatsFactor = 1.699189048,
                BloatsThreshold = 6,
                BlockedPointScore = 1.145912,
                ConnectedBlocksFactor = 1.979573916,
                ProbablityScore = false,
                RunOrBlockFactor = 1.358788917
            };
        }

        public static Config NoDoubles8Epochs()
        {
            return new Config
            {
                BloatsFactor = 1.27486242,
                BloatsThreshold = 3,
                BlockedPointScore = 0,
                ConnectedBlocksFactor = 0.762388608,
                ProbablityScore = false,
                RunOrBlockFactor = 0.452949965
            };
        }
    }
}

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
                BlotsFactor = BlotsFactor,
                BlotsFactorPassed = BlotsFactorPassed,
                BlotsThreshold = BlotsThreshold,
                RunOrBlockFactor = RunOrBlockFactor

            };
        }

        /// <summary>
        /// Blots are hitables checkers. Below this threshold does not reduce score.
        /// </summary>
        public int BlotsThreshold { get; set; }

        /// <summary>
        /// The point divided by this factor reduces score for blots.
        /// </summary>
        public double BlotsFactor { get; set; }

        /// <summary>
        /// The point divided by this factor reduces score for blots. When opponent has passes this point with all checker.
        /// </summary>
        public double BlotsFactorPassed { get; set; }

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
            return $"BF: {BlotsFactor.ToString("0.##")}  BFP: {BlotsFactorPassed}  BT: {BlotsThreshold}  CB: {ConnectedBlocksFactor.ToString("0.##")}  BP: {BlockedPointScore.ToString("0.##")}  RB: {RunOrBlockFactor.ToString("0.##")}";
        }

        public static Config Untrained()
        {
            return new Config
            {
                BlotsFactor = 1,
                BlotsFactorPassed = 1,
                BlotsThreshold = 0,
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
                BlotsFactor = 1.25,
                BlotsFactorPassed = 1.075,
                BlotsThreshold = 1,
                BlockedPointScore = 2.058432,
                ConnectedBlocksFactor = 0.191012719,
                ProbablityScore = false,
                RunOrBlockFactor = 0.174344897
            };
        }

        public static Config NoDoubles41Epochs()
        {
            return new Config
            {
                BlotsFactor = 1.747286362,
                BlotsThreshold = 14,
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
                BlotsFactor = 1.699189048,
                BlotsThreshold = 6,
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
                BlotsFactor = 1.27486242,
                BlotsThreshold = 3,
                BlockedPointScore = 0,
                ConnectedBlocksFactor = 0.762388608,
                ProbablityScore = false,
                RunOrBlockFactor = 0.452949965
            };
        }
    }
}

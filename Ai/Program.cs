﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai
{
    public class Program
    {
        public static void Main()
        {
            //Trainer.OptimizeBlotsThreshold();
            //Trainer.OptimizeBlotsFactor();
            //Trainer.RunStatic();
            //Trainer.ComparePassedBlocks();
            //Trainer.OptimizeConnectedBlocksFactor();
            //Trainer.OptimizeBlockedPointScore();
            //Trainer.OptimizeRunOrBlockFactor();

            Trainer.OptimizeAll();

            //Trainer.CompareConfigs();
        }
    }
}

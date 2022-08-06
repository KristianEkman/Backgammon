using Backend.Rules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ai
{
    public class Trainer
    {
        public Trainer(Config config = null)
        {
            Game = Game.Create(false);
            Black = new Engine(Game);
            White = new Engine(Game);
            if (config != null)
            {
                Black.Configuration = config.Clone();
                White.Configuration = config.Clone();
            }
        }

        private const double Improvement = 0.51;

        public Game Game { get; }

        public Engine Black { get; }
        public Engine White { get; }
        private Engine GetEngine()
        {
            if (Game.CurrentPlayer == Player.Color.Black)
                return Black;
            return White;
        }

        public static double RunMany(Trainer runner)
        {
            var start = DateTime.Now;
            var runs = 3000;

            var result = runner.PlayMany(runs);
            var time = DateTime.Now - start;

            var runsPs = runs / time.TotalSeconds;
            Console.WriteLine($"Games per second: {runsPs:0.#}");
            Console.WriteLine($"Starts: B {runner.Game.BlackStarts}, W {runner.Game.WhiteStarts}");
            return result.WhitePct;
        }

        public (double BlackPct, double WhitePct, int errors) PlayMany(int times)
        {
            int blackWins = 0;
            int whitekWins = 0;
            int errors = 0;
            Game.BlackStarts = 0;
            Game.WhiteStarts = 0;
            for (int i = 1; i <= times; i++)
            {
                Game.Reset();
                var winner = PlayGame();
                if (winner == Player.Color.Black)
                    blackWins++;
                else if (winner == Player.Color.White)
                    whitekWins++;
                else
                    errors++;
                if (i % 100 == 0)
                {
                    Console.CursorLeft = 0;
                    var blackPct = blackWins / (double)i;
                    var whitePct = whitekWins / (double)i;
                    Console.Write($"{i} Black: {blackPct:P} White: {whitePct:P} Errors: {errors}");
                    PreventSleep();
                }
            }
            Console.WriteLine();

            return (blackWins / (double)times, whitekWins / (double)times, errors);
        }

        public Player.Color? PlayGame()
        {
            Game.PlayState = Game.State.FirstThrow;
            //var switchCount = 0;
            while (Game.PlayState != Game.State.Ended)
            {
                while (Game.PlayState == Game.State.FirstThrow)
                {
                    Game.RollDice();
                }
                var engine = GetEngine();
                var moves = engine.GetBestMoves();
                foreach (var move in moves)
                {
                    if (move != null)
                        Game.MakeMove(move);
                }
                Game.SwitchPlayer();
                //switchCount++;
                //if (switchCount > 200)
                //{
                //    //Console.WriteLine("Rare Error. The game was for some reason stuck.");
                //    return null;
                //}
                if (Game.BlackPlayer.PointsLeft <= 0)
                {
                    Game.PlayState = Game.State.Ended;
                    return Player.Color.Black;
                }
                if (Game.WhitePlayer.PointsLeft <= 0)
                {
                    Game.PlayState = Game.State.Ended;
                    return Player.Color.White;
                }
                Game.NewRoll();
            }
            throw new ApplicationException("There must be a winner");
            //Console.WriteLine($"Winner {Game.Get}")
        }

        public static void RunStatic()
        {
            for (var t = 0; t < 10; t++)
            {
                var runner = new Trainer();
                Console.WriteLine($"=====Static=============");
                RunMany(runner);
            }
        }

        public static double OptimizeBlotsThreshold(int start = 1, int end = 10, Config config = null)
        {
            var best = 0d;
            var bestT = 0d;
            var delta = 1;

            for (double t = start; t < end; t += delta)
            {
                var runner = new Trainer(config);
                runner.White.Configuration.BlotsThreshold = (int)t;
                WriteConfigs(runner);                
                Console.WriteLine($"=====================");
                Console.WriteLine($"BlotsThreshold: {t}");
                var res = RunMany(runner);
                if (res > best && res > Improvement)
                {
                    best = res;
                    bestT = t;
                }
            }
            return bestT;
        }

        public static double OptimizeBlotsFactorPassed(double start = 1, double end = 10, Config config = null)
        {
            var best = 0d;
            var bestT = 0d;
            var delta = (end - start) / 4;

            for (var t = start; t < end; t += delta)
            {
                var runner = new Trainer(config);
                runner.White.Configuration.BlotsFactorPassed = t;
                WriteConfigs(runner);
                Console.WriteLine($"==================");
                Console.WriteLine($"BlotsFactorPassed: {t}");
                var res = RunMany(runner);
                if (res > best && res > Improvement)
                {
                    best = res;
                    bestT = t;
                }
            }
            return bestT;
        }

        public static double OptimizeBlotsFactor(double start = 1, double end = 10, Config config = null)
        {
            var best = 0d;
            var bestT = 0d;
            var delta = (end - start) / 4;

            for (var t = start; t < end; t += delta)
            {
                var runner = new Trainer(config);
                runner.White.Configuration.BlotsFactor = t;
                WriteConfigs(runner);
                Console.WriteLine($"==================");
                Console.WriteLine($"BlotsFactor: {t}");
                var res = RunMany(runner);
                if (res > best && res > Improvement)
                {
                    best = res;
                    bestT = t;
                }
            }
            return bestT;
        }

        public static double OptimizeConnectedBlocksFactor(double start = 1d, double end = 10d, Config config = null)
        {
            var best = 0d;
            var bestF = 0d;
            var delta = (end - start) / 4;
            for (var f = start; f < end; f += delta) // maximum at 3.6
            {
                var runner = new Trainer(config);
                runner.White.Configuration.ConnectedBlocksFactor = f;
                WriteConfigs(runner);
                Console.WriteLine($"===== ConnectedBlocksFactor {f}=========");
                var res = RunMany(runner);
                if (res > best && res > Improvement)
                {
                    best = res;
                    bestF = f;
                }
            }
            return bestF;
        }

        public static double OptimizeBlockedPointScore(double start = 0.5d, double end = 2d, Config config = null)
        {
            var best = 0d;
            var bestF = 0d;
            var delta = (end - start) / 4;

            for (var f = start; f < end; f += delta) // maximum at 3.6
            {
                var runner = new Trainer(config);
                runner.White.Configuration.BlockedPointScore = f;
                WriteConfigs(runner);
                Console.WriteLine($"===== BlockedPointScore {f}=========");
                var res = RunMany(runner);
                if (res > best && res > Improvement)
                {
                    best = res;
                    bestF = f;
                }
            }
            return bestF;
        }
        
        public static double OptimizeRunOrBlockFactor(double start = 0d, double end = 5d, Config config = null)
        {
            var best = 0d;
            var bestF = 0d;
            var delta = (end - start) / 4;

            for (var f = start; f < end; f += delta) // maximum at 3.6
            {
                var runner = new Trainer(config);
                runner.White.Configuration.RunOrBlockFactor = f;
                WriteConfigs(runner);
                Console.WriteLine($"===== RunOrBlockFactor {f}=========");
                var res = RunMany(runner);
                if (res > best && res > Improvement)
                {
                    best = res;
                    bestF = f;
                }
            }
            return bestF;
        }

        public static void OptimizeAll()
        {
            var config = Config.Trained();

            Console.WriteLine("*********************");
            Console.WriteLine(config.ToString());
            Console.WriteLine("*********************");
            var csvName = $"{Environment.CurrentDirectory}\\MaximizeAll{DateTime.Now:yyMMddHHmmss}.csv";
            Console.WriteLine(csvName);

            File.WriteAllText(csvName, "BlockedPointScore;ConnectedBlocksFactor;BlotsFactor;BlotsFactorPassed;BlotsThreshold;RunOrBlockFactor\n");

            const double lr = 0.1; // learning rate
            while (true)
            {
                var sBt = Math.Max(config.BlotsThreshold - 2, 0);
                var eBt = config.BlotsThreshold + 2;                
                var bt = OptimizeBlotsThreshold(sBt, eBt, config);                
                if (bt > 0)
                    config.BlotsThreshold = (int)bt;// config.BlotsThreshold + (bt - config.BlotsThreshold) / 2;
                
                var sBf = Math.Max(config.BlotsFactor - 0.5, 0.1);
                var eBf = config.BlotsFactor + 0.5;
                var bf = OptimizeBlotsFactor(sBf, eBf, config);
                if (bf > 0)
                    config.BlotsFactor += (bf - config.BlotsFactor) * lr;

                var sBfp = Math.Max(config.BlotsFactorPassed - 0.5, 0.1);
                var eBfp = config.BlotsFactorPassed + 0.5;
                var bfp = OptimizeBlotsFactorPassed(sBfp, eBfp, config);
                if (bfp > 0)
                    config.BlotsFactorPassed += (bfp - config.BlotsFactorPassed) * lr;


                var sCb = Math.Max(config.ConnectedBlocksFactor - 0.5, 0);
                var eCb = config.ConnectedBlocksFactor + 0.5;
                var cb = OptimizeConnectedBlocksFactor(sCb, eCb, config);
                if (cb > 0)
                    config.ConnectedBlocksFactor += (cb - config.ConnectedBlocksFactor) * lr;

                var sBp = Math.Max(config.BlockedPointScore - 0.5, 0);
                var eBp = config.BlockedPointScore + 0.5;
                var bp = OptimizeBlockedPointScore(sBp, eBp, config);
                if (bp > 0)
                    config.BlockedPointScore += (bp - config.BlockedPointScore) * lr;

                var sRb = Math.Max(config.RunOrBlockFactor - 0.5, 0);
                var eRb = config.RunOrBlockFactor + 0.5;
                var rb = OptimizeRunOrBlockFactor(sRb, eRb, config);
                if (rb > 0)
                    config.RunOrBlockFactor += (rb - config.RunOrBlockFactor) * lr;

                Console.WriteLine("*********************");
                Console.WriteLine(DateTime.Now.ToString() + " " + config.ToString());
                File.AppendAllText(csvName, $"{config.BlockedPointScore};{config.ConnectedBlocksFactor};{config.BlotsFactor};{config.BlotsFactorPassed};{config.BlotsThreshold};{config.RunOrBlockFactor}\n");
                Console.WriteLine("*********************");
            }
        }

        private static void WriteConfigs(Trainer runner)
        {
            Console.WriteLine("B: " + runner.Black.Configuration);
            Console.WriteLine("W: " + runner.White.Configuration);
        }

        public static void CompareConfigs()
        {
            var runner = new Trainer();
            runner.Black.Configuration = Config.NoDoubles41Epochs();
            runner.White.Configuration = Config.Untrained();
            _ = RunMany(runner);
        }

        static void PreventSleep()
        {
            // Prevent Idle-to-Sleep (monitor not affected) (see note above)
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }

    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }
}

using Backend.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai
{
    public class Runner
    {
        public Runner(Config config = null)
        {
            Game = Game.Create();
            Black = new Engine(Game);
            White = new Engine(Game);
            if (config != null)
            {
                Black.Configuration = config;
                White.Configuration = config.Clone();
            }
        }

        public Game Game { get; }

        public Engine Black { get; }
        public Engine White { get; }
        private Engine GetEngine()
        {
            if (Game.CurrentPlayer == Player.Color.Black)
                return Black;
            return White;
        }

        public static double RunMany(Runner runner)
        {
            var start = DateTime.Now;
            var runs = 2500;

            var result = runner.PlayMany(runs);
            var time = DateTime.Now - start;

            Console.WriteLine($"Black: {result.BlackPct.ToString("P")} White: {result.WhitePct.ToString("P")} Errors: {result.errors}");
            var runsPs = runs / time.TotalSeconds;
            Console.WriteLine($"Games per second: {runsPs.ToString("0.#")}");
            Console.WriteLine($"{runner.Game.BlackStarts}, {runner.Game.WhiteStarts}");
            return result.BlackPct;
        }

        public (double BlackPct, double WhitePct, int errors) PlayMany(int times)
        {
            int blackWins = 0;
            int whitekWins = 0;
            int errors = 0;
            Game.BlackStarts = 0;
            Game.WhiteStarts = 0;
            for (int i = 0; i < times; i++)
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
                    Console.Write(i);
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
                var runner = new Runner();
                // todo: enable Hitable for white but keep factor constant

                Console.WriteLine($"=====Static=============");
                RunMany(runner);
            }
        }

        public static int OptimizeHitableThreshold(int start = 2, int end = 20, Config config = null)
        {
            var best = 0d;
            var bestT = 0;
            for (int t = start; t < end; t++)
            {
                var runner = new Runner(config);
                runner.Black.Configuration.HitableThreshold = t;
                Console.WriteLine($"=====================");
                Console.WriteLine($"HitableThreshold: {t}");
                var res = RunMany(runner);
                if (res > best && res > 0.5)
                {
                    best = res;
                    bestT = t;
                }
            }
            return bestT;
        }

        public static double OptimizeHitableFactor(double start = 1, double end = 20, Config config = null)
        {
            var best = 0d;
            var bestT = 0d;

            for (var t = start; t < end; t += 1)
            {
                var runner = new Runner(config);
                // todo: enable Hitable for white but keep factor constant
                runner.Black.Configuration.HitableFactor = t; // 12.2 seems to be best so far.
                Console.WriteLine($"==================");
                Console.WriteLine($"HitableFactor: {t}");
                var res = RunMany(runner);
                if (res > best && res > 0.5)
                {
                    best = res;
                    bestT = t;
                }
            }
            return bestT;
        }

        public static double OptimizeConnectedBlocksFactor(double start = 3d, double end = 4d, Config config = null)
        {
            var best = 0d;
            var bestF = 0d;
            for (var f = start; f < end; f += 0.1d) // maximum at 3.6
            {
                var runner = new Runner(config);
                runner.Black.Configuration.ConnectedBlocksFactor = f;
                Console.WriteLine($"===== ConnectedBlocksFactor {f}=========");
                var res = RunMany(runner);
                if (res > best && res > 0.5)
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
            for (var f = start; f < end; f += 0.1d) // maximum at 3.6
            {
                var runner = new Runner(config);
                runner.Black.Configuration.BlockedPointScore = f;
                Console.WriteLine($"===== BlockedPointScore {f}=========");
                var res = RunMany(runner);
                if (res > best && res > 0.5)
                {
                    best = res;
                    bestF = f;
                }
            }
            return bestF;
        }

        public static void MaximizeAll()
        {
            var config = new Config();

            Console.WriteLine("*********************");
            Console.WriteLine(config.ToString());
            Console.WriteLine("*********************");
            while (true)
            {
                var sHf = Math.Max(config.HitableFactor - 5, 0);
                var eHf = config.HitableFactor + 5;
                var hf = OptimizeHitableFactor(sHf, eHf, config);
                if (hf > 0)
                    config.HitableFactor = config.HitableFactor + (hf - config.HitableFactor) / 2;

                var sHt = Math.Max(config.HitableThreshold - 5, 1);
                var eHt = config.HitableThreshold + 5;
                var ht = OptimizeHitableThreshold(sHt, eHt, config);
                if (ht > 0)
                    config.HitableThreshold = config.HitableThreshold + (ht - config.HitableThreshold) / 2;

                var sCb = Math.Max(config.ConnectedBlocksFactor - 1, 1);
                var eCb = config.ConnectedBlocksFactor + 1;
                var cb = OptimizeConnectedBlocksFactor(sCb, eCb, config);
                if (cb > 0)
                    config.ConnectedBlocksFactor = config.ConnectedBlocksFactor + (cb - config.ConnectedBlocksFactor) / 2;

                var sBp = Math.Max(config.BlockedPointScore - .5, 0);
                var eBp = config.BlockedPointScore + .5;
                var bp = OptimizeBlockedPointScore(sBp, eBp, config);
                if (bp > 0)
                    config.BlockedPointScore = config.BlockedPointScore + (bp - config.BlockedPointScore) / 2;

                Console.WriteLine("*********************");
                Console.WriteLine(config.ToString());
                Console.WriteLine("*********************");
            }
        }
    }
}

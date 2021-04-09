using System;
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
            RunMany();
        }

        public static void RunMany()
        {
            var runner = new Runner();
            var start = DateTime.Now;
            var runs = 1000;

            runner.Black.Configuration.HitableBad = true;

            var result = runner.PlayMany(runs);
            var time = DateTime.Now - start;

            Console.WriteLine($"Black: {result.BlackPct.ToString("P")} White: {result.WhitePct.ToString("P")}");
            var runsPs = runs / time.TotalSeconds;
            Console.WriteLine($"Games per second: {runsPs.ToString("0.#")}");
        }
    }
}

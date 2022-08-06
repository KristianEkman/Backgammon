using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rules
{
    public class Score
    {
        public static (int black, int white) NewScore(int blackScr, int whiteScr, int blackGames, int whiteGames, bool blackWon)
        {
            var blackK = GetK(blackGames);
            var whiteK = GetK(whiteGames);

            return EloRating(blackScr, whiteScr, blackK, whiteK, blackWon);
        }

        static double Probability(double rating1, double rating2)
        {
            return 1 / (1 + Math.Pow(10, (rating1 - rating2) / 400d));
        }

        private static double GetK(int games)
        {
            return 85 * Math.Exp(-0.1 * games) + 15;
        }

        static (int black, int white) EloRating(double black, double white, double blackK, double whiteK, bool blackWon)
        {
            double whiteProb = Probability(black, white);
            double blackProb = Probability(white, black);

            if (blackWon == true)
            {
                black += blackK * (1 - blackProb);
                white += whiteK * (0 - whiteProb);
            }
            else
            {
                black += blackK * (0 - blackProb);
                white += whiteK * (1 - whiteProb);
            }
            var b =(int) Math.Round(black);
            var w =(int) Math.Round(white);

            return (b, w) ;
        }
    }
}

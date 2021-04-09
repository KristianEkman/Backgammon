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
        public Runner()
        {
            Game = Game.Create();
            Black = new Engine(Game);
            White = new Engine(Game);
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

        public (double BlackPct, double WhitePct) PlayMany(int times)
        {
            int blackWins = 0;
            int whitekWins = 0;
            for (int i = 0; i < times; i++)
            {
                Game.Reset();
                var winner = PlayGame();
                if (winner == Player.Color.Black)
                    blackWins++;
                else
                    whitekWins++;
                Console.WriteLine(i);
            }
            return (blackWins / (double)times, whitekWins / (double)times);
        }

        public Player.Color PlayGame()
        {
            Game.PlayState = Game.State.FirstThrow;
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
            }
            throw new ApplicationException("There must be a winner");
            //Console.WriteLine($"Winner {Game.Get}")
        }


    }
}

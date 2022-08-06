using Backend.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ai
{
    public class Engine
    {
        public Engine(Game game)
        {
            EngineGame = game;
            Configuration = Config.Trained();
        }

        private Game EngineGame { get; }

        public Config Configuration { get; set; }

        public Move[] GetBestMoves()
        {
            Move[] bestMoveSequence = null;
            var bestScore = double.MinValue;
            var allSequences = GenerateMovesSequence(EngineGame);

            var oponent = EngineGame.OtherPlayer();
            var myColor = EngineGame.CurrentPlayer;
            const int inParallel = 2;

            var opt = new ParallelOptions { MaxDegreeOfParallelism = inParallel };
            Parallel.ForEach(allSequences, opt, (sequence) =>
            {
                var g = allSequences.IndexOf(sequence) % inParallel;
                var game = EngineGame.Clone();

                var localSequence = ToLocalSequence(sequence, game);

                var hits = DoSequence(localSequence, game);
                var score = 0d;
                if (Configuration.ProbablityScore)
                    score = -ProbabilityScore(oponent, game);
                else
                    score = EvaluatePoints(myColor, game) + EvaluateCheckers(myColor, game);

                UndoSequence(localSequence, hits, game);
                //Console.WriteLine($"Engine search {s} of {allSequences.Count}\t{score.ToString("0.##")}\t{sequence.BuildString()}");
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoveSequence = sequence;
                }
            });

            if (bestMoveSequence == null)
                return Array.Empty<Move>();
            
            if (myColor == Player.Color.Black)
                return bestMoveSequence.Where(m => m != null).OrderBy(m => m.From.BlackNumber).ToArray();

            return bestMoveSequence.Where(m => m != null).OrderBy(m => m.From.WhiteNumber).ToArray();
        }

        private static Move[] ToLocalSequence(Move[] sequence, Game game)
        {
            var moves = new Move[sequence.Length];
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] != null)
                {
                    moves[i] = new Move
                    {
                        From = game.Points[sequence[i].From.BlackNumber],
                        To = game.Points[sequence[i].To.BlackNumber],
                        Color = sequence[i].Color,

                    };
                }
            }
            return moves;
        }

        private static Stack<Checker> DoSequence(Move[] sequence, Game game)
        {
            var hits = new Stack<Checker>();
            foreach (var move in sequence)
            {
                if (move == null)
                    continue;
                var hit = game.MakeMove(move);
                hits.Push(hit);
            }
            game.SwitchPlayer();
            return hits;
        }

        private static void UndoSequence(Move[] sequence, Stack<Checker> hits, Game game)
        {
            game.SwitchPlayer();

            for (int i = sequence.Length - 1; i >= 0; i--)
                if (sequence[i] != null)
                    game.UndoMove(sequence[i], hits.Pop());
        }

        private static double EvaluatePoints(Player.Color myColor, Game game)
        {
            if (myColor == Player.Color.White) // Higher score for white when few checkers and black has many checkers left
                return game.BlackPlayer.PointsLeft - game.WhitePlayer.PointsLeft;
            else
                return game.WhitePlayer.PointsLeft - game.BlackPlayer.PointsLeft;
        }

        private double EvaluateCheckers(Player.Color myColor, Game game)
        {
            double score = 0;
            var inBlock = false;
            var blockCount = 0; // consequtive blocks
            var bt = Configuration.BlotsThreshold;
            var bf = Configuration.BlotsFactor;
            var bfp = Configuration.BlotsFactorPassed;
            var cbf = Configuration.ConnectedBlocksFactor;
            var bps = Configuration.BlockedPointScore;

            var other = myColor == Player.Color.Black ? Player.Color.White : Player.Color.Black;
            // Oponents checker closest to their bar. Relative to my point numbers.
            var opponentMax = game.Points.Where(p => p.Checkers.Any(c => c.Color == other))
                .Select(p => p.GetNumber(myColor)).Max();

            var myMin = game.Points.Where(p => p.Checkers.Any(c => c.Color == myColor))
                .Select(p => p.GetNumber(myColor)).Min();

            var allPassed = true;

            if (myMin < opponentMax)
            {
                for (int i = 1; i < 25; i++)
                {
                    // It is important to reverse looping for white.
                    var point = game.Points[i];
                    if (myColor == Player.Color.White)
                        point = game.Points[25 - i];

                    var pointNo = point.GetNumber(myColor);

                    // If all opponents checkers has passed this point, blots are not as bad.
                    allPassed = pointNo > opponentMax;

                    if (point.Block(myColor))
                    {
                        if (inBlock)
                            blockCount++;
                        else
                            blockCount = 1; // Start of blocks.
                        inBlock = true;
                    }
                    else // not a blocked point
                    {
                        if (inBlock)
                        {
                            score += Math.Pow(blockCount * bps, cbf);
                            blockCount = 0;
                        }
                        inBlock = false;
                        if (point.Blot(myColor) && point.GetNumber(myColor) > bt)
                            score -= point.GetNumber(myColor) / (allPassed ? bfp : bf);
                    }
                } // end of loop

                if (inBlock) // the last point.
                    score += Math.Pow(blockCount * bps, cbf);

                if (allPassed)
                    score += EvaluatePoints(myColor, game) * Configuration.RunOrBlockFactor;
            }
            else
            {
                // When both players has passed each other it is just better to move to home board and then bear off.
                score += game.GetHome(myColor).Checkers.Count * 100;
                score += game.Points.Count(p => p.GetNumber(myColor) > 18) * 50;
            }

            return score;
        }

        //Get the average score for current player rolling all possible combinations
        private double ProbabilityScore(Player.Color myColor, Game game)
        {
            var allDiceRoll = AllRolls();
            var scores = new List<double>();
            var oponent = myColor == Player.Color.Black ? Player.Color.White : Player.Color.Black;
            foreach (var (dice1, dice2) in allDiceRoll)
            {
                game.FakeRoll(dice1, dice2);
                var bestScore = double.MinValue;
                var seqs = GenerateMovesSequence(game);
                foreach (var s in seqs)
                {
                    var hits = DoSequence(s, game);
                    var score = EvaluatePoints(myColor, game) + EvaluateCheckers(myColor, game);
                    score -= EvaluateCheckers(oponent, game);
                    if (score > bestScore)
                        bestScore = score;
                    UndoSequence(s, hits, game);
                }
                int m = dice1 == dice2 ? 1 : 2; // dice roll with not same value on dices are twice as probable. 2 / 36, vs 1 / 36
                if (seqs.Any())
                    scores.Add(bestScore * m);
                // Get best score of each roll, and make an average.
                // some rolls are more probable, multiply them
                // some rolls will be blocked or partially blocked
            }
            if (!scores.Any())
                return -100000; // If player cant move, shes blocked. Thats bad.
            return scores.Average();
        }

        private static (int dice1, int dice2)[] _allRolls = null;
        private static (int dice1, int dice2)[] AllRolls()
        {
            if (_allRolls != null)
                return _allRolls;
            var list = new List<(int, int)>();
            for (int d1 = 1; d1 < 7; d1++)
                for (int d2 = 1; d2 < 7; d2++)
                {
                    if (!list.Contains((d1, d2)) && !list.Contains((d2, d1)))
                        list.Add((d1, d2));
                }
            _allRolls = list.ToArray();
            return _allRolls;
        }

        public static List<Move[]> GenerateMovesSequence(Game game)
        {
            var sequences = new List<Move[]>();
            var moves = new Move[game.Roll.Count];
            sequences.Add(moves);
            GenerateMovesSequence(sequences, moves, 0, game);

            // Special case. Sometimes the first dice is blocked, but can be moved after next dice
            if (sequences.Count == 1 && sequences[0].Any(move => move == null))
            {
                (game.Roll[1], game.Roll[0]) = (game.Roll[0], game.Roll[1]);
                GenerateMovesSequence(sequences, moves, 0, game);
            }

            // If there are move sequences with all moves not null, remove sequences that has some moves null.
            // (rule of backgammon that you have to use all dice if you can)
            if (sequences.Any(moves => moves.All(m => m != null)))
                sequences = sequences.Where(moves => moves.All(m => m != null)).Select(s => s).ToList();
            return sequences;
        }

        private static void GenerateMovesSequence(List<Move[]> sequences, Move[] moves, int diceIndex, Game game)
        {
            var current = game.CurrentPlayer;
            var bar = game.Bars[(int)current];
            var barHasCheckers = bar.Checkers.Any(c => c.Color == current);
            var dice = game.Roll[diceIndex];

            var points = barHasCheckers ? new[] { bar } :
                game.Points.Where(p => p.Checkers.Any(c => c.Color == current))
                //.OrderBy(p => p.GetNumber(current))
                .ToArray();

            // There seems to be a big advantage to evaluate points from lowest number.
            // If not reversing here, black will win 60 to 40 with same config.
            if (game.CurrentPlayer == Player.Color.White)
                Array.Reverse(points);

            foreach (var fromPoint in points)
            {
                var fromPointNo = fromPoint.GetNumber(game.CurrentPlayer);
                if (fromPointNo == 25)
                    continue;
                var toPoint = game.Points.SingleOrDefault(p => p.GetNumber(game.CurrentPlayer) == dice.Value + fromPointNo);
                if (toPoint != null && toPoint.IsOpen(game.CurrentPlayer)
                    && !toPoint.IsHome(game.CurrentPlayer)) // no creation of bearing off moves here. See next block.
                {
                    var move = new Move { Color = game.CurrentPlayer, From = fromPoint, To = toPoint };
                    //copy and make a new list for first dice
                    if (moves[diceIndex] == null)
                        moves[diceIndex] = move;
                    else // a move is already generated for this dice in this sequence. branch off a new.
                    {
                        var newMoves = new Move[game.Roll.Count];
                        Array.Copy(moves, newMoves, diceIndex);
                        newMoves[diceIndex] = move;
                        // For last checker identical sequences are omitted.
                        if (diceIndex < game.Roll.Count - 1 || !sequences.ContainsEntryWithAll(newMoves))
                        {
                            moves = newMoves;
                            sequences.Add(moves);
                        }
                    }

                    if (diceIndex < game.Roll.Count - 1) // Do the created move and recurse to next dice
                    {
                        var hit = game.MakeMove(move);
                        GenerateMovesSequence(sequences, moves, diceIndex + 1, game);
                        game.UndoMove(move, hit);
                    }
                }
                else if (game.IsBearingOff(game.CurrentPlayer))
                {
                    // The furthest away checker can be moved beyond home.
                    var minPoint = game.Points.Where(p => p.Checkers.Any(c => c.Color == game.CurrentPlayer)).OrderBy(p => p.GetNumber(game.CurrentPlayer)).First().GetNumber(game.CurrentPlayer);
                    var toPointNo = fromPointNo == minPoint ? Math.Min(25, fromPointNo + dice.Value) : fromPointNo + dice.Value;
                    toPoint = game.Points.SingleOrDefault(p => p.GetNumber(game.CurrentPlayer) == toPointNo);
                    if (toPoint != null && toPoint.IsOpen(game.CurrentPlayer))
                    {
                        var move = new Move { Color = game.CurrentPlayer, From = fromPoint, To = toPoint };
                        if (moves[diceIndex] == null)
                            moves[diceIndex] = move;
                        else
                        {
                            var newMoves = new Move[game.Roll.Count];
                            Array.Copy(moves, newMoves, diceIndex);
                            newMoves[diceIndex] = move;
                            // For last checker identical sequences are omitted.
                            if (diceIndex < game.Roll.Count - 1 || !sequences.ContainsEntryWithAll(newMoves))
                            {
                                moves = newMoves;
                                sequences.Add(moves);
                            }
                        }
                        if (diceIndex < game.Roll.Count - 1)
                        {
                            var hit = game.MakeMove(move);
                            GenerateMovesSequence(sequences, moves, diceIndex + 1, game);
                            game.UndoMove(move, hit);
                        }
                    }
                }
            }
        }

        private double Evaluate(Player.Color color, Game game)
        {
            var score = EvaluatePoints(color, game) + EvaluateCheckers(color, game);
            return score;
        }

        public bool AcceptDoubling()
        {
            if (!EngineGame.PlayersPassed())
                return true;

            var myScore = Evaluate(EngineGame.CurrentPlayer, EngineGame);
            var oponent = EngineGame.CurrentPlayer == Player.Color.Black ? Player.Color.White : Player.Color.Black;
            var otherScore = Evaluate(oponent, EngineGame);

            var oppPips = EngineGame.CurrentPlayer == Player.Color.White ?
                EngineGame.BlackPlayer.PointsLeft :
                EngineGame.WhitePlayer.PointsLeft;

            var k = (myScore - otherScore) / oppPips;

            return k > -0.25; // Just my best guess.
        }
    }
}

static class Extensions
{
    public static bool ContainsEntryWithAll(this List<Move[]> listOfList, Move[] match)
    {
        // searching for a list entry that contains all entries in match
        foreach (var list in listOfList)
        {
            var hasMove = true;
            foreach (var mv in match)
            {
                if (!list.Any(m => mv != null && m != null && m.From == mv.From && m.To == mv.To && m.Color == mv.Color))
                {
                    hasMove = false;
                    break;
                }
            }
            if (hasMove)
                return true;

        }
        return false;
    }

    public static string BuildString(this Move[] moves)
    {
        return string.Join(",", moves.Where(m => m != null).Select(m => m.ToString()).ToArray());
    }
}


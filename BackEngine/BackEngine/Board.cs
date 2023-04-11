using System.ComponentModel;
using System.Diagnostics;

namespace BackEngine;
public class Board
{
    public Board()
    {
        SetStartPosition();
    }
    /// <summary>
    /// Negative number is number of black checkers.
    /// Positive is white.
    /// Spots[0] is whites bar.
    /// Spots[25] is blacks bar.
    /// White moves positive direction, e.g. 1 -> 3
    /// Black moves oposite direction, e.g. 24 -> 22
    /// </summary>
    public sbyte[] Spots = new sbyte[26];
    public sbyte BlackHome;
    public sbyte WhiteHome;

    public const sbyte Black = -1;
    public const sbyte White = 1;

    public void SetStartPosition()
    {
        for (int i = 0; i < 26; i++)
            Spots[i] = 0;
        Spots[1] = 2;
        Spots[6] = -5;
        Spots[8] = -3;
        Spots[12] = 5;
        Spots[13] = -5;
        Spots[17] = 3;
        Spots[19] = 5;
        Spots[24] = -2;
    }

    /// <returns>True if oponent checker was hit.</returns>
    public bool DoMove(Move move)
    {
        var hit = false;
        var off = false;
        if (move.To == 25)
        {
            Debug.Assert(move.Side == White);
            WhiteHome++;
            off = true;
        }
        else if (move.To == 0)
        {
            Debug.Assert(move.Side == Black);
            BlackHome++;
            off = true;
        }
        else if (Spots[move.To] == -move.Side)
        {
            if (move.Side == Black)
                Spots[0]++;
            else
                Spots[25]--; // Black hit checkers have negative value, even on the bar.
            Spots[move.To] = 0;
            hit = true;
        }
        Spots[move.From] -= move.Side;

        if (!off)
            Spots[move.To] += move.Side;
        return hit;
    }

    public void UndoMove(Move move, bool hit)
    {
        var off = false;
        if (move.To == 25)
        {
            Debug.Assert(move.Side == White);
            WhiteHome--;
            off = true;
        }
        else if (move.To == 0)
        {
            Debug.Assert(move.Side == Black);
            BlackHome--;
            off = true;
        }
        else if (hit)
        {
            if (move.Side == Black)
                Spots[0]--;
            else
                Spots[25]++;
            Spots[move.To] = (sbyte)-move.Side;
        }
        Spots[move.From] += move.Side;
        if (!off && !hit)
            Spots[move.To] -= move.Side;
    }

    public void CreateMoves(Generation gen, sbyte side)
    {
        gen.HashSet.Clear();
        gen.GeneratedCount = 0;
        gen.HasFullSets = false;

        if (side == White)
        {
            CreateMovesWhite(gen, 0);
            if (gen.Dice.Length == 2) // dice are not the same
            {
                var dice0 = gen.Dice[0];
                gen.Dice[0] = gen.Dice[1];
                gen.Dice[1] = dice0;                
                CreateMovesWhite(gen, 0);
            }
        }
        else
        {
            CreateMovesBlack(gen, 0);
            if (gen.Dice.Length == 2) // dice are not the same
            {
                var dice0 = gen.Dice[0];
                gen.Dice[0] = gen.Dice[1];
                gen.Dice[1] = dice0;
                CreateMovesBlack(gen, 0);
            }
        }

        if (gen.HasPartialSets && gen.HasFullSets)
        {
            gen.MoveSets = gen.MoveSets.Take(gen.GeneratedCount).ToList().Where(ms => ms.Take(gen.Dice.Length).All(m => m.Side != 0)).ToArray();
            gen.GeneratedCount = gen.MoveSets.Length;
        }
    }


    private void CreateMovesWhite(Generation gen, int currentDiceIdx)
    {
        var bearingOff = true;
        var firstCheckerIndex = 0;
        for (int i = 0; i < 25; i++)
        {
            if (Spots[i] > 0)
            {
                firstCheckerIndex = i;
                bearingOff = (i >= 19);
                break;
            }
        }

        var canMove = false;
        for (int i = firstCheckerIndex; i < 25; i++)
        {
            if (Spots[i] < 1) // no white here
                continue;
            var to = i + gen.Dice[currentDiceIdx];
            if (to > 24)
            {
                if (!bearingOff)
                    break;
                if (to > 25 && i != firstCheckerIndex)
                    break;
                to = 25;
            }
            else
            {
                if (Spots[to] < -1)
                    continue; // blocked

                if (i > 0 && Spots[0] > 0)
                    break;
            }
            canMove = true;
            Move move;
            move.From = (byte)i;
            move.To = (byte)to;
            move.Side = White;
            gen.MoveSets[gen.GeneratedCount][currentDiceIdx] = move;

            if (currentDiceIdx == gen.Dice.Length - 1) //last dice
            {
                // if not all dice are found the set should be generated anyway
                gen.GeneratedCount++;
                for (int d = 0; d < currentDiceIdx; d++)
                {
                    gen.MoveSets[gen.GeneratedCount][d] =
                        gen.MoveSets[gen.GeneratedCount - 1][d];
                }

                if (!gen.KeepSet())
                    gen.GeneratedCount--;
                else
                    gen.HasFullSets = true;
                continue;
            }
            // Recurse to next dice
            var hit = DoMove(move);
            CreateMovesWhite(gen, currentDiceIdx + 1);
            UndoMove(move, hit);
        }

        if (!canMove && currentDiceIdx > 0)
        {
            gen.GeneratedCount++;
            gen.HasPartialSets = true;

            for (int d = 0; d < currentDiceIdx; d++)
            {
                gen.MoveSets[gen.GeneratedCount][d] =
                    gen.MoveSets[gen.GeneratedCount - 1][d];
            }

            if (!gen.KeepSet())
                gen.GeneratedCount--;
        }
    }

    private void CreateMovesBlack(Generation gen, int currentDiceIdx)
    {
        var bearingOff = true;
        var firstCheckerIndex = 25;
        for (int i = 25; i > 0; i--)
        {
            if (Spots[i] < 0)
            {
                bearingOff = (i <= 6);
                firstCheckerIndex = i;
                break;
            }
        }

        var canMove = false;
        for (int i = firstCheckerIndex; i > 0; i--)
        {
            if (Spots[i] > -1) //no black here
                continue;
            var to = i - gen.Dice[currentDiceIdx];
            if (to < 1)
            {
                if (!bearingOff)
                    break;
                if (to < 0 && i != firstCheckerIndex)
                    break;
                to = 0;
            }
            else
            {
                if (Spots[to] > 1)
                    continue; // blocked

                if (i < 25 && Spots[25] < 0)
                    break;
            }
            canMove = true;
            Move move;
            move.From = (byte)i;
            move.To = (byte)to;
            move.Side = Black;
            gen.MoveSets[gen.GeneratedCount][currentDiceIdx] = move;

            if (currentDiceIdx == gen.Dice.Length - 1) //last dice
            {
                gen.GeneratedCount++;
                for (int d = 0; d < currentDiceIdx; d++)
                {
                    gen.MoveSets[gen.GeneratedCount][d] =
                        gen.MoveSets[gen.GeneratedCount - 1][d];
                }
                if (!gen.KeepSet())
                    gen.GeneratedCount--;
                else
                    gen.HasFullSets = true;
                continue;
            }
            // Recurse to next dice
            var hit = DoMove(move);
            CreateMovesBlack(gen, currentDiceIdx + 1);
            UndoMove(move, hit);
        }

        if (!canMove && currentDiceIdx > 0)
        {
            gen.HasPartialSets = true;
            gen.GeneratedCount++;
            for (int d = 0; d < currentDiceIdx; d++)
            {
                gen.MoveSets[gen.GeneratedCount][d] =
                    gen.MoveSets[gen.GeneratedCount - 1][d];
            }

            if (!gen.KeepSet())
                gen.GeneratedCount--;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < Spots.Length; i++)
        {
            Spots[i] = 0;
        }
        BlackHome = 0;
        WhiteHome = 0;
    }
}

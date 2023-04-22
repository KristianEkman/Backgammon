using System.Diagnostics;
using System.Text;

namespace BackEngine;
public class Board
{
    public Board()
    {
        SetStartPosition();
    }

    private static Randoms Randoms = new();
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

    public const int Black = -1;
    public const int White = 1;

    public int WhitePip { get; set; }
    public int BlackPip { get; set; }

    public int FirstWhite { get; set; }
    public int FirstBlack { get; set; }

    public uint Hash { get; set; }

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

        CountPips();
        SetFirstWhite();
        SetFirstBlack();
        InitHash();
    }

    private void InitHash()
    {
        var hash = Randoms.Start;
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < Spots[i] + 15; j++)
            {
                hash ^= Randoms.PlaceCount[i, j];
            }
        }
        Hash = hash;
    }

    private void SetFirstBlack()
    {
        for (int i = 25; i >= 1; i--)
        {
            if (Spots[i] < 0)
            {
                FirstBlack = i;
                break;
            }
        }
    }

    private void SetFirstWhite()
    {
        for (int i = 0; i < 26; i++)
        {
            if (Spots[i] > 0)
            {
                FirstWhite = i;
                break;
            }
        }
    }

    public void CountPips()
    {
        var black = 0;
        var white = 0;
        for (int i = 0; i < 26; i++)
        {
            var x = Spots[i];
            if (x < 0)
                black += i * -Spots[i]; // Remeber, black cant have checkers on Spot[0], thats whites bar, and vv.
            if (x > 0)
                white += (25 - i) * Spots[i];
        }
        WhitePip = white;
        BlackPip = black;
    }

    /// <returns>True if oponent checker was hit.</returns>
    public Undid DoMove(Move move)
    {
        Undid undid;
        undid.FirstBlack = (byte)FirstBlack;
        undid.FirstWhite = (byte)FirstWhite;
        undid.WhitePip = (ushort)WhitePip;
        undid.BlackPip = (ushort)BlackPip;
        undid.Hash = Hash;

#if DEBUG
        AssertBoard(move);
#endif
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
        else if (Spots[move.To] == -move.Side) //one checker of oposite color on that spot.
        {
            if (move.Side == Black)
            {
                Spots[0]++;
                UpdateHash(0);
            }
            else
            {
                Spots[25]--; // Black hit checkers have negative value, even on the bar.
                UpdateHash(25);
            }
            UpdateHash(move.To);
            Spots[move.To] = 0;
            hit = true;
        }
        UpdateHash(move.From);
        Spots[move.From] -= move.Side;

        if (!off)
        {
            Spots[move.To] += move.Side;
            UpdateHash(move.To);
        }

        if (move.Side == White)
        {
            WhitePip -= (move.To - move.From);
            if (hit)
            {
                BlackPip += move.To;
                FirstBlack = 25;
            }
            if (move.From == FirstWhite && Spots[FirstWhite] == 0)
                SetFirstWhite();
        }
        else
        {
            BlackPip -= (move.From - move.To);
            if (hit)
            {
                WhitePip += 25 - move.To;
                FirstWhite = 0;
            }

            if (move.From == FirstBlack && Spots[FirstBlack] == 0)
                SetFirstBlack();
        }
        undid.Hit = hit;
        return undid;
    }

    private void UpdateHash(int spotIdx)
    {
        // Offsetting checker count by 15 since they range from -15 to 15 --> 0 to 30
        Hash ^= Randoms.PlaceCount[spotIdx, Spots[spotIdx] + 15];
    }

    private void AssertBoard(Move move)
    {
        Debug.Assert(Spots[move.From] * move.Side > 0, "No checker of correct color found on FromSpot");
        if (move.Side == White)
            Debug.Assert(Spots[move.To] > -2, "ToSpot was blocked");
        if (move.Side == Black)
            Debug.Assert(Spots[move.To] < 2, "ToSpot was blocked");
        AssertSum();

        // todo assert off - moves
    }

    private void AssertSum()
    {
        var blackSum = BlackHome;
        var whiteSum = WhiteHome;
        for (int i = 0; i < 26; i++)
        {
            if (Spots[i] > 0)
                whiteSum += Spots[i];
            if (Spots[i] < 0)
                blackSum -= Spots[i];
        }
        Debug.Assert(whiteSum == 15, "White checker(s) missing");
        Debug.Assert(blackSum == 15, "Black checker(s) missing");
    }

    public void UndoMove(Move move, Undid undid)
    {
        var off = false;
        var hit = undid.Hit;
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

        WhitePip = undid.WhitePip;
        BlackPip = undid.BlackPip;
        FirstWhite = undid.FirstWhite;
        FirstBlack = undid.FirstBlack;
        Hash = undid.Hash;

#if DEBUG
        AssertSum();
#endif
    }

    public void CreateMoves(Generation gen, int side)
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

            if (currentDiceIdx == gen.Dice.Length - 1) //Last dice
            {
                // If not all dice are found the set should be generated anyway
                gen.GeneratedCount++;
                for (int d = 0; d < currentDiceIdx; d++)
                {
                    gen.MoveSets[gen.GeneratedCount][d] =
                        gen.MoveSets[gen.GeneratedCount - 1][d];
                }

                var undid = DoMove(move);
                if (!gen.KeepSet(Hash))
                {
                    gen.GeneratedCount--;
#if DEBUG
                    Debug.Assert(!gen.DebugKeepSet(this.ToString()));
#endif
                }
                else
                {
                    gen.HasFullSets = true;
#if DEBUG
                    Debug.Assert(gen.DebugKeepSet(this.ToString()));
#endif
                }
                UndoMove(move, undid);
                continue;
            }

            // Recurse to next dice
            var hit = DoMove(move);
            CreateMovesWhite(gen, currentDiceIdx + 1);
            UndoMove(move, hit);
        }

        // Special case when not all dice can be used
        if (!canMove && currentDiceIdx > 0)
        {
            gen.GeneratedCount++;
            gen.HasPartialSets = true;

            for (int d = 0; d < currentDiceIdx; d++)
            {
                gen.MoveSets[gen.GeneratedCount][d] =
                    gen.MoveSets[gen.GeneratedCount - 1][d];
            }
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

                var undid = DoMove(move);
                if (!gen.KeepSet(Hash))
                {
                    gen.GeneratedCount--;
#if DEBUG
                    Debug.Assert(!gen.DebugKeepSet(this.ToString()));
#endif
                }
                else
                {
                    gen.HasFullSets = true;
#if DEBUG
                    Debug.Assert(gen.DebugKeepSet(this.ToString()));
#endif
                }
                UndoMove(move, undid);
                continue;
            }
            // Recurse to next dice
            var hit = DoMove(move);
            CreateMovesBlack(gen, currentDiceIdx + 1);
            UndoMove(move, hit);
        }

        // Special case when not all dice can be used
        if (!canMove && currentDiceIdx > 0)
        {
            gen.HasPartialSets = true;
            gen.GeneratedCount++;
            for (int d = 0; d < currentDiceIdx; d++)
            {
                gen.MoveSets[gen.GeneratedCount][d] =
                    gen.MoveSets[gen.GeneratedCount - 1][d];
            }
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

    /// <summary>
    /// Positive good for White, neg good for Black.
    /// </summary>
    /// <returns>Score</returns>
    public int GetScore()
    {
        const int pipFactor = 1;
        const int blotFactor = -5;

        const int blockFactor = 3;
        const int bigStackFactor = -1;

        var score = BlackPip * pipFactor - WhitePip * pipFactor;
        for (int i = 1; i < 25; i++)
        {
            var checkers = Spots[i]; // neg for black
            if (checkers == 0)
                continue;
            if (checkers > 0) // white
            {
                if (checkers == 1)
                {
                    if (i < FirstBlack)
                        score += blotFactor;
                }
                else // a block
                {
                    score += blockFactor;
                    if (checkers > 3)
                        score += bigStackFactor;
                }
            }
            else // Black
            {
                if (checkers == -1)
                {
                    if (i > FirstWhite)
                        score -= blotFactor;
                }
                else // a block
                {
                    score -= blockFactor;
                    if (checkers < -3)
                        score -= bigStackFactor;
                }
            }
        }
        return score;
    }

    public override string ToString()
    {
        var s = string.Join(' ', Spots) + "";
        return s;
    }
}

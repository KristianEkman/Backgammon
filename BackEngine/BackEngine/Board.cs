namespace BackEngine;
public class Board
{
    public Board()
    {
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class Grid
{
    public enum Status { Black = -1, None = 0, White = 1 }

    private Index index;
    private Status stat;
    private Piece piece;

    public Grid()
    {
        index = new Index(-1, -1);
        stat = Status.None;
    }

    public Status GetStat()
    {
        return stat;
    }

    public Index GetIndex()
    {
        return index;
    }

    public void SetStat(Status stat)
    {
        this.stat = stat;
    }

    public void SetPiece(Piece piece)
    {
        this.piece = piece;
    }

    public void SetIndex(int i, int j)
    {
        this.index = new Index(i, j);
    }

    public void Print()
    {
        UnityEngine.Debug.LogWarning(index.Item1 + " / " + index.Item2 + " / " + stat.ToString());
    }
}

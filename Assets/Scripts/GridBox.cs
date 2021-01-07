using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class GridBox
{
    public enum Status { Black = -1, None = 0, White = 1 }

    private Index index;
    private Status stat;
    public Piece piece;

    public GridBox()
    {
        index = new Index(-1, -1);
        stat = Status.None;
    }

    public void Flip()
    {
        var newC = (Status) ((int) stat * -1);
        
        piece.FlipAnim(newC);
        
        SetStat(newC);
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
}

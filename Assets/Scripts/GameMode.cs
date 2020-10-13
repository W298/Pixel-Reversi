using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class Grid
{
    public enum Status { None, Black, White }

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

public class Matrix
{
    private Grid[,] data;

    public Matrix()
    {
        data = new Grid[8, 8];
        
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                data[i, j] = new Grid();
                data[i, j].SetIndex(i, j);
            }
        }
    }

    public Grid[,] GetData()
    {
        return data;
    }

    public Grid GetGrid(Index index)
    {
        return data[index.Item1, index.Item2];
    }
}

public class GameMode : MonoBehaviour
{
    public GameObject pieceObj;
    
    public Matrix matrix;

    private PlayerController PC;

    public GameObject CreatePieceObj(Grid.Status playerColor, Index index)
    {
        Vector3 pos = PC.IndexToVector2(index);
        pos.z = -1;
        
        var obj = Instantiate(pieceObj, pos, Quaternion.identity);
        obj.name = index.Item1 + " / " + index.Item2;
        
        var grid = matrix.GetGrid(index);
        var piece = obj.GetComponent<Piece>();

        grid.SetStat(playerColor);
        grid.SetPiece(piece);
        piece.SetColor(playerColor);

        return obj;
    }

    private void InitStaticPieces()
    {
        PC.InitIndexPos();
        
        CreatePieceObj(Grid.Status.White, new Index(3, 3));
        CreatePieceObj(Grid.Status.White, new Index(4, 4));
        CreatePieceObj(Grid.Status.Black, new Index(4, 3));
        CreatePieceObj(Grid.Status.Black, new Index(3, 4));
    }

    void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            
        matrix = new Matrix();
        
        InitStaticPieces();
    }
    
    void Update()
    {
        
    }
}

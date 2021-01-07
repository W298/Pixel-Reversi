using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Index = System.Tuple<int, int>;
public class Matrix
{
    private GridBox[,] data;

    public Matrix()
    {
        data = new GridBox[8, 8];
        
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                data[i, j] = new GridBox();
                data[i, j].SetIndex(i, j);
            }
        }
    }

    public GridBox[,] GetData()
    {
        return data;
    }

    public GridBox GetGrid(Index index)
    {
        return data[index.Item1, index.Item2];
    }
}

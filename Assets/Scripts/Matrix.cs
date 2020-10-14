using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Index = System.Tuple<int, int>;
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

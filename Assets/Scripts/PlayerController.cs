using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class PlayerController : MonoBehaviour
{
    public GameObject cursorObj;

    public Grid.Status playerColor;

    private Camera MainCamera;
    private GameMode GameMode;

    private GameObject cursor;
    private Vector2 curPos;
    private Vector2 snapPos;
    private Index curIndex;
    
    private float[] xIndexPos;
    private float[] yIndexPos;

    float SnapGrid(float value, float snapSize = 0.5f)
    {
        if (value < 0)
        {
            return Mathf.Round(Mathf.Abs(value / snapSize)) * snapSize * -1;
        }
        else
        {
            return Mathf.Round(value / snapSize) * snapSize;
        }
    }
    
    public Index Vector2ToIndex(Vector2 snapPos)
    {
        var i = Array.FindIndex(xIndexPos, x => x == snapPos.x);
        var j = Array.FindIndex(yIndexPos, y => y == snapPos.y);

        return new Index(i, j);
    }

    public Vector2 IndexToVector2(Index index)
    {
        return new Vector2(xIndexPos[index.Item1], yIndexPos[index.Item2]);
    }

    bool CheckPieceValid(Grid.Status color, Index index)
    {
        var pieceList = GetCrossPieces(index);
        var sameColorList = pieceList.Where(piece => piece.GetStat() == color && 
                                                     !IsAdjacent(index, piece.GetIndex()) &&
                                                     GetCrossPieces(index, piece.GetIndex()).
                                                         Any(p => p.GetStat() != Grid.Status.None &&
                                                                  p.GetStat() != color));

        return sameColorList.Count() != 0;
    }

    public static bool IsAdjacent(Index i1, Index i2)
    {
        for (var x = i1.Item1 - 1; x <= i1.Item1 + 1; x++)
        {
            for (var y = i1.Item2 - 1; y <= i1.Item2 + 1; y++)
            {
                if (x == i2.Item1 && y == i2.Item2) return true;
            }
        }

        return false;
    }

    IEnumerable<Grid> GetCrossPieces(Index i1, Index i2, bool excludeNone = true)
    {
        var list = new List<Grid>();

        if (i1.Item1 == i2.Item1)
        {
            var start = Mathf.Min(i1.Item2, i2.Item2);
            var end = Mathf.Max(i1.Item2, i2.Item2);
            for (var j = start; j <= end; j++)
            {
                var grid = GameMode.matrix.GetGrid(new Index(i1.Item1, j));
                if (grid.GetStat() != Grid.Status.None || !excludeNone)
                {
                    list.Add(grid);
                }
            }

            return list;
        }
        
        if (i1.Item2 == i2.Item2)
        {
            var start = Mathf.Min(i1.Item1, i2.Item1);
            var end = Mathf.Max(i1.Item1, i2.Item1);
            for (var i = start; i <= end; i++)
            {
                var grid = GameMode.matrix.GetGrid(new Index(i, i1.Item2));
                if (grid.GetStat() != Grid.Status.None || !excludeNone)
                {
                    list.Add(grid);
                }
            }

            return list;
        }

        var dx = i1.Item1 - i2.Item1;
        var dy = i1.Item2 - i2.Item2;
        
        if (Mathf.Abs(dx) == Mathf.Abs(dy))
        {
            if (Mathf.Sign(dx) == Mathf.Sign(dy))
            {
                var b = i1.Item1 < i2.Item1;
                var l = Mathf.Abs(i1.Item1 - i2.Item1);

                for (var d = 1; d <= l; d++)
                {
                    var grid = GameMode.matrix.GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d, 
                        (b ? i1.Item2 : i2.Item2) + d)
                    );
                    
                    if (grid.GetStat() != Grid.Status.None || !excludeNone)
                    {
                        list.Add(grid);
                    }
                }

                return list;
            }
            else
            {
                var b = i1.Item1 < i2.Item1;
                var l = Mathf.Abs(i1.Item1 - i2.Item1);

                for (var d = 1; d <= l; d++)
                {
                    var grid = GameMode.matrix.GetGrid(new Index(
                        (b ? i1.Item1 : i2.Item1) + d, 
                        (b ? i1.Item2 : i2.Item2) - d)
                    );
                    
                    if (grid.GetStat() != Grid.Status.None || !excludeNone)
                    {
                        list.Add(grid);
                    }
                }

                return list;
            }
        }

        return null;
    }
    
    IEnumerable<Grid> GetCrossPieces(Index index, bool excludeNone = true)
    {
        var list = new List<Grid>();
        var (cx, cy) = index;
        
        for (var i = 0; i < 8; i++)
        {
            if (i == cx) continue;
            
            var grid = GameMode.matrix.GetGrid(new Index(i, index.Item2));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }

        for (var j = 0; j < 8; j++)
        {
            if (j == cy) continue;
            
            var grid = GameMode.matrix.GetGrid(new Index(index.Item1, j));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy + d >= 8) continue;
            
            var grid = GameMode.matrix.GetGrid(new Index(cx + d, cy + d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
            
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy - d < 0) continue;
            
            var grid = GameMode.matrix.GetGrid(new Index(cx - d, cy - d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy - d < 0) continue;
            
            var grid = GameMode.matrix.GetGrid(new Index(cx + d, cy - d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy + d >= 8) continue;
            
            var grid = GameMode.matrix.GetGrid(new Index(cx - d, cy + d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }

        return list;
    }

    void InitComp()
    {
        MainCamera = GetComponentInChildren<Camera>();
        GameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();
    }

    public void InitIndexPos()
    {
        xIndexPos = new float[8];
        yIndexPos = new float[8];

        xIndexPos[0] = -1.5f;
        for (var i = 1; i < 8; i++)
        {
            xIndexPos[i] = xIndexPos[i - 1] + 0.5f;
        }

        yIndexPos[0] = 2.0f;
        for (var i = 1; i < 8; i++)
        {
            yIndexPos[i] = yIndexPos[i - 1] - 0.5f;
        }
    }

    void UpdatePos()
    {
        curPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        snapPos = new Vector2(SnapGrid(curPos.x), SnapGrid(curPos.y));
        curIndex = Vector2ToIndex(snapPos);
        
        cursor.transform.position = snapPos;
    }

    void PlacePiece()
    {
        GameMode.CreatePieceObj(playerColor, curIndex);
    }

    void ShowPossible()
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (CheckPieceValid(playerColor, new Index(i, j)))
                {
                    var v = IndexToVector2(new Index(i, j));
                    Instantiate(cursorObj, new Vector3(v.x, v.y, -1), Quaternion.identity);
                }
            }
        }
    }

    void Start()
    {
        InitComp();
        InitIndexPos();
        
        cursor = Instantiate(cursorObj, Vector3.zero, Quaternion.identity);
    }
    
    void Update()
    {
        UpdatePos();
        
        if (Input.GetMouseButtonDown(0))
        {
            PlacePiece();
            
            ShowPossible();
        }
    }
}

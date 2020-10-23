using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class GameMode : MonoBehaviour
{
    public GameObject curObj;
    public GameObject indcObj;

    public GameObject pieceObj;
    
    public Matrix matrix;

    private PlayerController playerC;

    private List<GameObject> indicObjs = new List<GameObject>();
    
    private static float[] xIndexPos;
    private static float[] yIndexPos;
    
    public static Index Vector2ToIndex(Vector2 snapPos)
    {
        var i = Array.FindIndex(xIndexPos, x => x == snapPos.x);
        var j = Array.FindIndex(yIndexPos, y => y == snapPos.y);

        return new Index(i, j);
    }

    public static Vector2 IndexToVector2(Index index)
    {
        return new Vector2(xIndexPos[index.Item1], yIndexPos[index.Item2]);
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

    public void PlacePiece(Grid.Status playerColor, Index index, bool isStatic = false)
    {
        if (!CheckPieceValid(playerColor, index, true) && !isStatic) return;
        
        Vector3 pos = IndexToVector2(index);
        pos.z = -1;
        
        var obj = Instantiate(pieceObj, pos, Quaternion.identity);
        obj.name = index.Item1 + " / " + index.Item2;
        
        var grid = matrix.GetGrid(index);
        var piece = obj.GetComponent<Piece>();

        grid.SetStat(playerColor);
        grid.SetPiece(piece);
        piece.SetColor(playerColor);

        if (isStatic) return;

        ClearIndicObjs();
        playerC.playerColor = (Grid.Status) ((int) playerC.playerColor * -1);
        ShowPossibleLocation(playerC.playerColor);
    }

    private void ClearIndicObjs()
    {
        foreach (var obj in indicObjs)
        {
            Destroy(obj);
        }
        
        indicObjs.Clear();
    }

    private void InitStaticPieces()
    {
        PlacePiece(Grid.Status.White, new Index(3, 3), true);
        PlacePiece(Grid.Status.White, new Index(4, 4), true);
        PlacePiece(Grid.Status.Black, new Index(4, 3), true);
        PlacePiece(Grid.Status.Black, new Index(3, 4), true);
    }

    private void ShowPossibleLocation(Grid.Status color)
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (CheckPieceValid(color, new Index(i, j)))
                {
                    var v = IndexToVector2(new Index(i, j));
                    var indc = Instantiate(indcObj, new Vector3(v.x, v.y, -1), Quaternion.identity);
                    indicObjs.Add(indc);
                }
            }
        }
    }

    bool CheckPieceValid(Grid.Status color, Index index, bool execute = false)
    {
        if (matrix.GetGrid(index).GetStat() != Grid.Status.None) return false;
        
        var pieceList = GetCrossPieces(index);
        var sameColorPieces = pieceList.Where(piece => piece.GetStat() == color);
        var able = false;

        foreach (var piece in sameColorPieces)
        {
            if (IsAdjacent(index, piece.GetIndex())) continue;

            var crossList = GetCrossPieces(index, piece.GetIndex(), false);
            var revColorPieces = crossList.Where(p => p.GetStat() == (Grid.Status) ((int) color * -1));
            var b = crossList.Any(p => p.GetStat() == Grid.Status.None || p.GetStat() == color);

            if (revColorPieces.Count() != 0 && !b)
            {
                if (execute)
                {
                    foreach (var revColorPiece in revColorPieces)
                    {
                        revColorPiece.Flip();
                    }
                }

                able = true;
            }
        }

        return able;
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

        if (IsAdjacent(i1, i2)) return null;

        if (i1.Item1 == i2.Item1)
        {
            var start = Mathf.Min(i1.Item2, i2.Item2);
            var end = Mathf.Max(i1.Item2, i2.Item2);
            for (var j = start + 1; j <= end - 1; j++)
            {
                var grid = matrix.GetGrid(new Index(i1.Item1, j));
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
            for (var i = start + 1; i <= end - 1; i++)
            {
                var grid = matrix.GetGrid(new Index(i, i1.Item2));
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

                for (var d = 1; d < l; d++)
                {
                    var grid = matrix.GetGrid(new Index(
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

                for (var d = 1; d < l; d++)
                {
                    var grid = matrix.GetGrid(new Index(
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
            
            var grid = matrix.GetGrid(new Index(i, index.Item2));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }

        for (var j = 0; j < 8; j++)
        {
            if (j == cy) continue;
            
            var grid = matrix.GetGrid(new Index(index.Item1, j));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy + d >= 8) continue;
            
            var grid = matrix.GetGrid(new Index(cx + d, cy + d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
            
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy - d < 0) continue;
            
            var grid = matrix.GetGrid(new Index(cx - d, cy - d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy - d < 0) continue;
            
            var grid = matrix.GetGrid(new Index(cx + d, cy - d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy + d >= 8) continue;
            
            var grid = matrix.GetGrid(new Index(cx - d, cy + d));
            if (grid.GetStat() != Grid.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }

        return list;
    }

    void Start()
    {
        matrix = new Matrix();
        playerC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        InitIndexPos();
        InitStaticPieces();
        ShowPossibleLocation(playerC.playerColor);
        
        Debug.LogWarning((int)Grid.Status.Black);
    }
    
    void Update()
    {
        
    }
}

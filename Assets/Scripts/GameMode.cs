using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Index = System.Tuple<int, int>;

public class GameMode : MonoBehaviour
{
    private static GameMode instance = null;
    public static GameMode Instance => instance != null ? instance : null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GridBox.Status humanColor = GridBox.Status.White;
    public GridBox.Status comColor = GridBox.Status.Black;
    
    public GameObject curObj;
    public GameObject indcObj;
    public GameObject pieceObj;
    
    private Matrix matrix;
    private Text blackCountText = null;
    private Text whiteCountText = null;
    private Image blackSelectImg = null;
    private Image whiteSelectImg = null;
    private Text indicatorText = null;
    private SwitchButton switchButton = null;

    private List<GameObject> indicObjs = new List<GameObject>();

    public List<Piece> blackPieces;
    public List<Piece> whitePieces;
    
    private static float[] xIndexPos;
    private static float[] yIndexPos;
    private bool stopInputForPass = false;
    private bool gameEnd = false;

    public enum GameType { Computer, Human }
    private GameType currentGameType;
    
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

    public void ChangeGameType(GameType type)
    {
        currentGameType = type;
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

    void PassTurn()
    {
        ClearIndicObjs();
        PlayerController.Instance.playerColor = (GridBox.Status) ((int) PlayerController.Instance.playerColor * -1);
        
        if (PlayerController.Instance.playerColor == GridBox.Status.Black)
        {
            blackSelectImg.enabled = true;
            whiteSelectImg.enabled = false;
        }
        else
        {
            whiteSelectImg.enabled = true;
            blackSelectImg.enabled = false;
        }

        if (currentGameType == GameType.Human || PlayerController.Instance.playerColor == humanColor)
        {
            ShowPossibleLocation(PlayerController.Instance.playerColor);
        }
        else
        {
            PlayerController.Instance.DisableInput();
            StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(0.65f);
                AI.Instance.Execute(comColor);
                
                PlayerController.Instance.EnableInput();
            }
        }
    }

    public void PlacePiece(GridBox.Status playerColor, Index index, bool isStatic = false)
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
        
        piece.SetParentGrid(grid);
        piece.SetColor(playerColor);

        if (playerColor == GridBox.Status.Black)
            blackPieces.Add(piece);
        else whitePieces.Add(piece);
        
        UpdateCountUI();

        if (isStatic) return;
        
        PassTurn();
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
        PlacePiece(GridBox.Status.White, new Index(3, 3), true);
        PlacePiece(GridBox.Status.White, new Index(4, 4), true);
        PlacePiece(GridBox.Status.Black, new Index(4, 3), true);
        PlacePiece(GridBox.Status.Black, new Index(3, 4), true);

        if (PlayerController.Instance.playerColor == GridBox.Status.Black)
        {
            blackSelectImg.enabled = true;
            whiteSelectImg.enabled = false;
        }
        else
        {
            whiteSelectImg.enabled = true;
            blackSelectImg.enabled = false;
        }
    }

    public List<Index> GetPossibleLocation(GridBox.Status color)
    {
        var list = new List<Index>();
        
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (CheckPieceValid(color, new Index(i, j)))
                {
                    list.Add(new Index(i, j));
                }
            }
        }

        if (list.Count != 0)
        {
            return list;
        }
        else
        {
            if (blackPieces.Count + whitePieces.Count == 8*8)
            {
                if (blackPieces.Count < whitePieces.Count)
                    indicatorText.text = "White Win";
                else if (blackPieces.Count > whitePieces.Count)
                    indicatorText.text = "Black Win";
                else
                    indicatorText.text = "Drew";

                gameEnd = true;

                return null;
            }

            indicatorText.text = "Pass";
            stopInputForPass = true;
            StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(2.0f);
                indicatorText.text = "";
                stopInputForPass = false;
            }
            
            PassTurn();
            return null;
        }
    }

    private void ShowPossibleLocation(GridBox.Status color)
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

        if (indicObjs.Count == 0)
        {
            if (blackPieces.Count + whitePieces.Count == 8*8)
            {
                if (blackPieces.Count < whitePieces.Count)
                    indicatorText.text = "White Win";
                else if (blackPieces.Count > whitePieces.Count)
                    indicatorText.text = "Black Win";
                else
                    indicatorText.text = "Drew";

                gameEnd = true;

                return;
            }

            indicatorText.text = "Pass";
            stopInputForPass = true;
            StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(2.0f);
                indicatorText.text = "";
                stopInputForPass = false;
            }
            
            PassTurn();
        }
    }

    void UpdateCountUI()
    {
        string bText;
        string wText;
        
        if (blackPieces.Count < 10)
        {
            bText = "0" + blackPieces.Count.ToString();
        }
        else
        {
            bText = blackPieces.Count.ToString();
        }

        if (whitePieces.Count < 10)
        {
            wText = "0" + whitePieces.Count.ToString();
        }
        else
        {
            wText = whitePieces.Count.ToString();
        }

        blackCountText.text = bText;
        whiteCountText.text = wText;
    }

    private int GetDistance(Index i1, Index i2)
    {
        return Mathf.Max(Mathf.Abs(i1.Item1 - i2.Item1), Mathf.Abs(i1.Item2 - i2.Item2));
    }

    bool CheckPieceValid(GridBox.Status color, Index index, bool execute = false)
    {
        if (matrix.GetGrid(index).GetStat() != GridBox.Status.None) return false;
        
        var pieceList = GetCrossPieces(index);
        var sameColorPieces = pieceList.Where(piece => piece.GetStat() == color);
        var able = false;

        foreach (var piece in sameColorPieces)
        {
            if (IsAdjacent(index, piece.GetIndex())) continue;

            var crossList = GetCrossPieces(index, piece.GetIndex(), false);
            var revColorPieces = crossList.Where(p => p.GetStat() == (GridBox.Status) ((int) color * -1));
            var b = crossList.Any(p => p.GetStat() == GridBox.Status.None || p.GetStat() == color);

            if (revColorPieces.Count() != 0 && !b)
            {
                if (execute)
                {
                    foreach (var revColorPiece in revColorPieces)
                    {
                        revColorPiece.Flip();
                        if (revColorPiece.GetStat() == GridBox.Status.Black)
                        {
                            blackPieces.Add(revColorPiece.piece);
                            whitePieces.Remove(revColorPiece.piece);
                        }
                        else
                        {
                            whitePieces.Add(revColorPiece.piece);
                            blackPieces.Remove(revColorPiece.piece);
                        }
                        
                        UpdateCountUI();
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

    IEnumerable<GridBox> GetCrossPieces(Index i1, Index i2, bool excludeNone = true)
    {
        var list = new List<GridBox>();

        if (IsAdjacent(i1, i2)) return null;

        if (i1.Item1 == i2.Item1)
        {
            var start = Mathf.Min(i1.Item2, i2.Item2);
            var end = Mathf.Max(i1.Item2, i2.Item2);
            for (var j = start + 1; j <= end - 1; j++)
            {
                var grid = matrix.GetGrid(new Index(i1.Item1, j));
                if (grid.GetStat() != GridBox.Status.None || !excludeNone)
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
                if (grid.GetStat() != GridBox.Status.None || !excludeNone)
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
                    
                    if (grid.GetStat() != GridBox.Status.None || !excludeNone)
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
                    
                    if (grid.GetStat() != GridBox.Status.None || !excludeNone)
                    {
                        list.Add(grid);
                    }
                }

                return list;
            }
        }

        return null;
    }
    
    IEnumerable<GridBox> GetCrossPieces(Index index, bool excludeNone = true)
    {
        var list = new List<GridBox>();
        var (cx, cy) = index;
        
        for (var i = 0; i < 8; i++)
        {
            if (i == cx) continue;
            
            var grid = matrix.GetGrid(new Index(i, index.Item2));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }

        for (var j = 0; j < 8; j++)
        {
            if (j == cy) continue;
            
            var grid = matrix.GetGrid(new Index(index.Item1, j));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy + d >= 8) continue;
            
            var grid = matrix.GetGrid(new Index(cx + d, cy + d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
            
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy - d < 0) continue;
            
            var grid = matrix.GetGrid(new Index(cx - d, cy - d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx + d >= 8 || cy - d < 0) continue;
            
            var grid = matrix.GetGrid(new Index(cx + d, cy - d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }
        
        for (var d = 1; d <= 7; d++)
        {
            if (cx - d < 0 || cy + d >= 8) continue;
            
            var grid = matrix.GetGrid(new Index(cx - d, cy + d));
            if (grid.GetStat() != GridBox.Status.None || !excludeNone)
            {
                list.Add(grid);
            }
        }

        return list;
    }

    void Start()
    {
        matrix = new Matrix();
        blackCountText = GameObject.FindGameObjectWithTag("Black_Text").GetComponent<Text>();
        whiteCountText = GameObject.FindGameObjectWithTag("White_Text").GetComponent<Text>();
        blackSelectImg = GameObject.FindGameObjectWithTag("Black_Select").GetComponent<Image>();
        whiteSelectImg = GameObject.FindGameObjectWithTag("White_Select").GetComponent<Image>();
        indicatorText = GameObject.FindGameObjectWithTag("Indicator").GetComponent<Text>();
        switchButton = GameObject.FindGameObjectWithTag("SwitchButton").GetComponent<SwitchButton>();
        
        switchButton.SetModeCom();
        
        InitIndexPos();
        InitStaticPieces();
        ShowPossibleLocation(PlayerController.Instance.playerColor);
    }
    
    void Update()
    {
        var hasAnimatingPiece = blackPieces.Any(p => p.isAnimating) || whitePieces.Any(p => p.isAnimating);

        if (hasAnimatingPiece || stopInputForPass || gameEnd)
        {
            PlayerController.Instance.DisableInput();
        }
        else
        {
            PlayerController.Instance.EnableInput();
        }
    }
}

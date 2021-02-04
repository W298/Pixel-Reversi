using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance = null;
    public static PlayerController Instance => instance != null ? instance : null;

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
    
    public GridBox.Status playerColor;

    private Camera MainCamera;

    private GameObject cursor;
    private Vector2 curPos;
    private Vector2 snapPos;
    private Index curIndex;

    private bool isInputEnabled = true;
    private bool needDisableCursor = false;
    public bool forceDisableCursor = false;

    public void EnableInput()
    {
        isInputEnabled = true;
    }

    public void DisableInput()
    {
        isInputEnabled = false;
    }

    public void EnableCursor()
    {
        if (!forceDisableCursor)
            cursor.SetActive(true);
    }

    public void DisableCursor()
    {
        if (cursor || needDisableCursor)
        {
            cursor.SetActive(false);
            needDisableCursor = false;
        }
        else
        {
            needDisableCursor = true;
        }
    }

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

    void InitComp()
    {
        MainCamera = GetComponentInChildren<Camera>();
    }

    void UpdatePos()
    {
        curPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (curPos.x <= -1.75 || curPos.y <= -1.75 || curPos.x >= 2.25 || curPos.y >= 2.25)
        {
            DisableCursor();
            return;
        }
        
        EnableCursor();
        
        snapPos = new Vector2(SnapGrid(curPos.x), SnapGrid(curPos.y));
        curIndex = GameMode.Vector2ToIndex(snapPos);
        
        cursor.transform.position = snapPos;
    }

    void Start()
    {
        InitComp();
        cursor = Instantiate(GameMode.Instance.curObj, Vector3.zero, Quaternion.identity);
    }
    
    void Update()
    {
        UpdatePos();
        
        if (Input.GetMouseButtonDown(0) && isInputEnabled)
        {
            GameMode.Instance.PlacePiece(playerColor, curIndex);
        }
    }
}

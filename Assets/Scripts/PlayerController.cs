using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;

using Index = System.Tuple<int, int>;

public class PlayerController : MonoBehaviour
{
    public GridBox.Status playerColor;

    private Camera MainCamera;
    private GameMode GameMode;

    private GameObject cursor;
    private Vector2 curPos;
    private Vector2 snapPos;
    private Index curIndex;

    private bool isInputEnabled = true;

    public void EnableInput()
    {
        isInputEnabled = true;
    }

    public void DisableInput()
    {
        isInputEnabled = false;
    }

    void EnableCursor()
    {
        cursor.SetActive(true);
    }

    void DisableCursor()
    {
        cursor.SetActive(false);
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
        GameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();
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
        cursor = Instantiate(GameMode.curObj, Vector3.zero, Quaternion.identity);
    }
    
    void Update()
    {
        UpdatePos();
        
        if (Input.GetMouseButtonDown(0) && isInputEnabled)
        {
            GameMode.PlacePiece(playerColor, curIndex);
        }
    }
}

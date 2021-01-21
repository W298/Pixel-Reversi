using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    private Text comBtnTxt;
    private Text humanBtnTxt;
    private GameObject ColorSelect;

    void Start()
    {
        var txts = GetComponentsInChildren<Text>().ToArray();
        comBtnTxt = txts[0];
        humanBtnTxt = txts[1];
        
        ColorSelect = GameObject.FindGameObjectWithTag("ColorSelect");
        if (ColorSelect == null) Debug.LogError("ColorSelect not found!");
    }

    public void SetModeCom()
    {
        comBtnTxt.color = Color.white;
        humanBtnTxt.color = Color.gray;
        
        ColorSelect.SetActive(true);

        GameMode.Instance.ChangeGameType(GameMode.GameType.Computer);
    }

    public void SetModeHuman()
    {
        comBtnTxt.color = Color.gray;
        humanBtnTxt.color = Color.white;
        
        ColorSelect.SetActive(false);

        GameMode.Instance.ChangeGameType(GameMode.GameType.Human);
    }

    public void SetWhite()
    {
        GameMode.Instance.humanColor = GridBox.Status.White;
        GameMode.Instance.comColor = GridBox.Status.Black;
        
        if (PlayerController.Instance.playerColor == GridBox.Status.Black)
            AI.Instance.Execute(GridBox.Status.Black);
        
        ColorSelect.SetActive(false);
    }

    public void SetBlack()
    {
        GameMode.Instance.humanColor = GridBox.Status.Black;
        GameMode.Instance.comColor = GridBox.Status.White;
        
        if (PlayerController.Instance.playerColor == GridBox.Status.White)
            AI.Instance.Execute(GridBox.Status.White);
        
        ColorSelect.SetActive(false);
    }
}

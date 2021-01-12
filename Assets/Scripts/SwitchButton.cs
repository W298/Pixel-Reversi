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

    private GameMode GameMode;

    void Start()
    {
        var txts = GetComponentsInChildren<Text>().ToArray();
        comBtnTxt = txts[0];
        humanBtnTxt = txts[1];

        GameMode = GameObject.FindGameObjectWithTag("GameMode").GetComponent<GameMode>();
    }

    public void SetModeCom()
    {
        comBtnTxt.color = Color.white;
        humanBtnTxt.color = Color.gray;

        GameMode.ChangeGameType(GameMode.GameType.Computer);
    }

    public void SetModeHuman()
    {
        comBtnTxt.color = Color.gray;
        humanBtnTxt.color = Color.white;

        GameMode.ChangeGameType(GameMode.GameType.Human);
    }
}

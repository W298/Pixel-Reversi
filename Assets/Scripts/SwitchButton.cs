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

    void Start()
    {
        var txts = GetComponentsInChildren<Text>().ToArray();
        comBtnTxt = txts[0];
        humanBtnTxt = txts[1];
    }

    public void SetModeCom()
    {
        comBtnTxt.color = Color.white;
        humanBtnTxt.color = Color.gray;

        GameMode.Instance.ChangeGameType(GameMode.GameType.Computer);
    }

    public void SetModeHuman()
    {
        comBtnTxt.color = Color.gray;
        humanBtnTxt.color = Color.white;

        GameMode.Instance.ChangeGameType(GameMode.GameType.Human);
    }
}

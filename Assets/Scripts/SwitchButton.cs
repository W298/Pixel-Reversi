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

    void DisableUI()
    {
        GameMode.Instance.disableInputForSelect = true;
        PlayerController.Instance.DisableCursor();
        PlayerController.Instance.forceDisableCursor = true;
    }

    void EnableUI()
    {
        GameMode.Instance.disableInputForSelect = false;
        PlayerController.Instance.EnableCursor();
        PlayerController.Instance.forceDisableCursor = false;
    }

    public void SetModeCom()
    {
        comBtnTxt.color = Color.white;
        humanBtnTxt.color = Color.gray;
        
        ColorSelect.SetActive(true);

        DisableUI();
        GameMode.Instance.ChangeGameType(GameMode.GameType.Computer);
    }

    public void SetModeHuman()
    {
        ColorSelect.SetActive(false);
        EnableUI();

        comBtnTxt.color = Color.gray;
        humanBtnTxt.color = Color.white;
        
        GameMode.Instance.ChangeGameType(GameMode.GameType.Human);
    }

    public void SetWhite()
    {
        GameMode.Instance.humanColor = GridBox.Status.White;
        GameMode.Instance.comColor = GridBox.Status.Black;
        
        if (PlayerController.Instance.playerColor == GridBox.Status.Black)
            AI.Instance.Execute(GridBox.Status.Black);
        
        ColorSelect.SetActive(false);
        
        EnableUI();
        GameMode.Instance.StartGame();
    }

    public void SetBlack()
    {
        GameMode.Instance.humanColor = GridBox.Status.Black;
        GameMode.Instance.comColor = GridBox.Status.White;
        
        if (PlayerController.Instance.playerColor == GridBox.Status.White)
            AI.Instance.Execute(GridBox.Status.White);
        
        ColorSelect.SetActive(false);
        
        EnableUI();
        GameMode.Instance.StartGame();
    }
}

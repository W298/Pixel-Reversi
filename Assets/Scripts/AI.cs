using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private static AI instance = null;
    public static AI Instance => instance != null ? instance : null;

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

    private List<List<int>> gridWeight = new List<List<int>>();

    void InitWeight()
    {
        var l0 = new List<int>() {50, -10, 15, 15, 15, 15, -10, 50};
        var l1 = new List<int>() {-10, -20, -10, -10, -10, -20, -10};
        var l2 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l3 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l4 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l5 = new List<int>() {15, -10, 1, 1, 1, 1, -10, 15};
        var l6 = new List<int>() {-10, -20, -10, -10, -10, -20, -10};
        var l7 = new List<int>() {50, -10, 15, 15, 15, 15, -10, 50};
        
        gridWeight.Add(l0);
        gridWeight.Add(l1);
        gridWeight.Add(l2);
        gridWeight.Add(l3);
        gridWeight.Add(l4);
        gridWeight.Add(l5);
        gridWeight.Add(l6);
        gridWeight.Add(l7);
    }

    public void Execute(GridBox.Status comColor)
    {
        var possibleLocs = GameMode.Instance.GetPossibleLocation(comColor);
        
        var max = -100;
        var selectedLoc = new Tuple<int, int>(-1, -1);
        
        foreach (var loc in possibleLocs)
        {
            if (gridWeight[loc.Item1].IndexOf(loc.Item2) > max)
            {
                max = gridWeight[loc.Item1].IndexOf(loc.Item2);
                selectedLoc = loc;
            }
        }

        GameMode.Instance.PlacePiece(comColor, selectedLoc);
    }

    private void Start()
    {
        InitWeight();
    }
}

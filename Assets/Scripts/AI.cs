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
        var sr = new StreamReader("./Assets/Scripts/GridWeight.csv");
        while (!sr.EndOfStream)
        {
            var s = sr.ReadLine();
            var temp = s.Split(',').ToList();
            var temp_list = temp.Select(value => int.Parse(value)).ToList();
            gridWeight.Add(temp_list);
        }
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

        GameMode.Instance.PlacePiece(GridBox.Status.Black, selectedLoc);
    }

    private void Start()
    {
        InitWeight();
    }
}

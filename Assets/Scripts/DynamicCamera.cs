using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    private Camera cam = null;

    float between(float min, float value, float max) 
    {
        return Mathf.Min(Mathf.Max(value, min), max);
    }
    
    void Start()
    {
        cam = this.gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        var size = between(3.2f, 3.2f * ((float)1000 / (float)Screen.width), 10);
        cam.orthographicSize = Convert.ToSingle(size);
    }
}

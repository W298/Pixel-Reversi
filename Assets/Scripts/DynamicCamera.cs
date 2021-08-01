using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    private Camera camera = null;
    private float initialValue;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = this.gameObject.GetComponent<Camera>();
        initialValue = camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.width <= Screen.height)
        {
            camera.orthographicSize = Convert.ToSingle(initialValue * ((float)Screen.height / (float)Screen.width) * 0.8);
        }
    }
}

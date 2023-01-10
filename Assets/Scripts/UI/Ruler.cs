using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler : MonoBehaviour
{
    void OnGUI()
    {
        for (int i = 0; i < Screen.width / 30f; i++)
        {
            GUI.Label(new Rect( 30 * i, 0, Screen.width, Screen.height), i.ToString());
        }
    }

    void Update()
    {
        Debug.Log("Test: " + Screen.width);
        for (int i = 0; i < Screen.width / 30f; i++)
        {
            Debug.DrawLine(new Vector2(i + 0.5f, 0), new Vector2(i + 0.5f, 5), Color.red);
            //GUI.Label(new Rect(30 * i, 0, Screen.width, Screen.height), i.ToString());
        }
        
    }
}

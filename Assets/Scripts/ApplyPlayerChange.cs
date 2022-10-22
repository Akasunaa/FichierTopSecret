using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public static class ApplyPlayerChange
{
    static RegexOptions options;


    public static void VisualChange(string name, string value, GameObject go)
    {
        name = "size";
        value = "10";

        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
        //Color
        if (Regex.IsMatch(name, "colou?r", options))
        {
            Color(value, go);
        }

        if (Regex.IsMatch(name, "size", options))
        {
            //nul
        }

        if (Regex.IsMatch(name, "position", options))
        {
            //nul
        }

    }

    static private void Color(string value, GameObject go)
    {
        string[] colors = { "black", "blue", "cyan", "gray", "green", "grey", "magenta", "red", "white", "yellow" };
        foreach (var colorValue in colors)
        {
            if (Regex.IsMatch(value, colorValue, options))
            {
                value = colorValue;
                break;
            }
        }
        if (ColorUtility.TryParseHtmlString(value, out Color color))
        {
            Light2D[] lights = go.GetComponentsInChildren<Light2D>();
            if (lights.Length > 0)
            {
                foreach (var light in lights)
                {
                    light.color = color;
                }
            }
            else if (go.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color = color;
            }
        }
    }


}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class ApplyPlayerChange
{

    static string[] colors = { "black", "blue", "cyan", "gray", "green", "grey", "magenta", "red", "white", "yellow" };

    public static void VisualChange(string name, string value, GameObject go)
    {
        name = "size";
        value = "10";

        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
        //Color
        if (Regex.IsMatch(name, "colou?r", options))
        {
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

                if (go.TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.color = color;
                }
            }
        }

        //Size - a etoffer
        if (Regex.IsMatch(name, "size", options))
        {
            go.transform.localScale = new Vector2(Int32.Parse(value), Int32.Parse(value));
            Debug.Log(Int32.Parse(value));
        }

        //location - todo
        if (Regex.IsMatch(name, "location", options))
        {

        }

        //idee
        //speed (npc) location rotation 

    }

    

    public static int LevenshteinDistance(string subject, string model)
    {
        if ( Mathf.Min(subject.Length, model.Length) == 0)
        {
            return Mathf.Max(subject.Length, model.Length);
        } else if (subject[0] == model[0])
        {
            return LevenshteinDistance(subject.Substring(1), model.Substring(1));
        } else
        {
            return 1 + Mathf.Min(LevenshteinDistance(subject.Substring(1), model),
                             LevenshteinDistance(subject, model.Substring(1)),
                             LevenshteinDistance(subject.Substring(1), model.Substring(1)));
        }
    }
}


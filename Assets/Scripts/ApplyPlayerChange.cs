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
    static string[] propertyNames = { "position", "color", "size", "direction", "power" };


    public static void VisualChange(string name, string value, GameObject go)
    {
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

    

    public static int LevenshteinDistance(string subject, string model)
    {
        string subSubject = subject.Substring(1);
        string subModel = model.Substring(1);

        if ( Mathf.Min(subject.Length, model.Length) == 0)
        {
            return Mathf.Max(subject.Length, model.Length);
        } else if (subject[0] == model[0])
        {
            return LevenshteinDistance(subSubject, subModel);
        } else
        {
            return 1 + Mathf.Min(LevenshteinDistance(subSubject, model),
                             LevenshteinDistance(subject, subModel),
                             LevenshteinDistance(subSubject, subModel));
        }
    }


    /**
     * Methods takes in a string typed by the player and compares it with all the property names registered.
     * Returns the closest one if their levenshtein distance is below the threshold.
     */
    public static string PropertyNameValidation(string propertyNameInput, int inclusiveValidationThreshold = 2)
    {
        string closestPropertyName = propertyNames[0];
        int levenshteinDistance = 10;

        foreach(string propertyName in propertyNames)
        {
            int currentLevenshteinDistance = LevenshteinDistance(propertyNameInput, propertyName);
            if (currentLevenshteinDistance < levenshteinDistance)
            {
                closestPropertyName = propertyName;
                levenshteinDistance = currentLevenshteinDistance;
            }
        }

        if (levenshteinDistance <= inclusiveValidationThreshold)
        {
            return closestPropertyName;
        } else
        {
            return propertyNameInput;
        }
    }
}


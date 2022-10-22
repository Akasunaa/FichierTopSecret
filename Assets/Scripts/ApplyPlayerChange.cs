using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class ApplyPlayerChange
{

    static string[] colors = { "black", "blue", "cyan", "gray", "green", "grey", "magenta", "red", "white", "yellow" };
    static string[] propertyNames = { "position", "color", "size", "direction", "power" };

    public static void VisualChange(string name, string value, GameObject go)
    {
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
        } else
        {
            string subSubject = subject.Substring(1);
            string subModel = model.Substring(1);

            if (subject[0] == model[0])
            {
                return LevenshteinDistance(subSubject, subModel);
            } else
            {
                return 1 + Mathf.Min(LevenshteinDistance(subSubject, model),
                                 LevenshteinDistance(subject, subModel),
                                 LevenshteinDistance(subSubject, subModel));
            }
        }
    }


    /**
     * Methods takes in a string typed by the player and compares it with all the property names registered.
     * Returns the closest one if their levenshtein distance is below the threshold.
     */
    public static string PropertyNameValidation(string propertyNameInput, int inclusiveValidationThreshold = 2)
    {
        if (propertyNameInput.Length == 0) return "";

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


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
    // names of the properties we interact with
    static string[] propertyNames = { "position", "color", "size", "direction", "power" };
    static string[] truthyPropertyValues = { "true", "on", "yes", "enabled", "activated"};
    static string[] falsyPropertyValues = { "false", "off", "no", "disabled", "deactivated"};


    public static void VisualChange(string name, string value, GameObject go)
    {
        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

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
            Position(value, go);
        }

    }

    static private void Position(string value, GameObject go)
    {
        // pattern that we want into the value string - correct ex: (0,0) 
        const string number = @"(\-?)\d+";
        const string separator = @"[\ \;\,]+";

        if (!Regex.IsMatch(value, number + separator + number, options)) return;
            
        // here we just want to extract the different decimals inside the value 
        var decodedCoordinates = Regex.Matches(value, number, options);

        float xTarget = float.Parse(decodedCoordinates[0].Value);
        float yTarget = float.Parse(decodedCoordinates[1].Value);

        //Debug.Log($"Entered coordinates are : ({xTarget}, {yTarget})");

        // creating final target vector and injecting it in go position
        Vector3Int targetPosition = new Vector3Int((int) xTarget, (int) yTarget, 0);

        // check if the position is occupied
        if (!Utils.CheckPresenceOnTile(SceneData.Instance.grid, SceneData.Instance.grid.GetCellCenterWorld(targetPosition)))
        {
            // move the object
            go.transform.position = SceneData.Instance.grid.GetCellCenterWorld(targetPosition);
            // update order in layer
            Utils.UpdateOrderInLayer(go);
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

    /**
     * Calculate the levenshtein distance between two words
     */
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
        if (propertyNameInput.Length == 0) return string.Empty;

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

    public static string BooleanPropertyValueValidation(string propertyValueInput)
    {
        if (propertyValueInput.Length == 0) return propertyValueInput;

        // if truthy values contain the input return true
        if (Array.IndexOf(truthyPropertyValues, propertyValueInput) > -1)
        {
            return "true";
        }

        // if falsy values contain the input return false
        if (Array.IndexOf(falsyPropertyValues, propertyValueInput) > -1)
        {
            return "false";
        }

        return propertyValueInput;
    }
}


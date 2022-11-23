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
    static RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
    // names of the properties we interact with
    private static readonly string[] PropertyNames = { "position", "color", "size", "direction", "power" };
    private static readonly string[] TruthyPropertyValues = { "true", "on", "yes", "enabled", "activated"};
    private static readonly string[] FalsyPropertyValues = { "false", "off", "no", "disabled", "deactivated"};

    public static void VisualChange(string name, object value, GameObject go)
    {
        switch (name)
        {
            case "position":
                if (ModifiableController.TryParse(value, out Vector2Int pos))
                {
                    if (!Utils.CheckPresenceOnTile(SceneData.Instance.grid, SceneData.Instance.grid.GetCellCenterWorld((Vector3Int) pos)))
                    {
                        // move the object
                        go.transform.position = SceneData.Instance.grid.GetCellCenterWorld((Vector3Int) pos);
                        // update order in layer
                        Utils.UpdateOrderInLayer(go);
                    } 
                }
                else
                {
                    Debug.LogWarning("Position badly written : " + value);
                }
                break;
            case "color":
                if (ModifiableController.TryParse(value, out Color color))
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
                else
                {
                    Debug.LogWarning("Color badly written : " + value);
                }
                break;
        }
    }

    private static Vector2Int? CheckPosition(string value)
    {
        // pattern that we want into the value string - correct ex: (0,0) 
        const string number = @"(\-?)\d+";
        const string separator = @"[\ \;\,]+";

        if (!Regex.IsMatch(value, number + separator + number, options)) return null;
            
        // here we just want to extract the different decimals inside the value 
        var decodedCoordinates = Regex.Matches(value, number, options);

        float xTarget = float.Parse(decodedCoordinates[0].Value);
        float yTarget = float.Parse(decodedCoordinates[1].Value);
        
        return new Vector2Int((int) xTarget, (int) yTarget);
    }

    private static Color? CheckColor(string value)
    {
        string[] colors = { "black", "blue", "cyan", "gray", "green", "grey", "magenta", "red", "white", "yellow" };
        foreach (String colorValue in colors)
        {
            if (Regex.IsMatch(value, colorValue, options))
            {
                value = colorValue;
                break;
            }
        }
        if (ColorUtility.TryParseHtmlString(value, out Color color))
        {
            return color;
        }

        return null;
    }

    /**
     * Calculate the levenshtein distance between two words
     */
    private static int LevenshteinDistance(string subject, string model)
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

        string closestPropertyName = PropertyNames[0];
        int levenshteinDistance = 10;

        foreach(string propertyName in PropertyNames)
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

    private static bool? CheckBool(string propertyValueInput)
    {
        if (propertyValueInput.Length == 0) return null;

        // if truthy values contain the input return true
        if (Array.IndexOf(TruthyPropertyValues, propertyValueInput) > -1)
        {
            return true;
        }

        // if falsy values contain the input return false
        if (Array.IndexOf(FalsyPropertyValues, propertyValueInput) > -1)
        {
            return false;
        }

        return null;
    }

    public static object ObjectFromValue(string value)
    {
        Vector2Int? pos = CheckPosition(value);
        if (pos != null) return pos;
        
        Color? c = CheckColor(value);
        if (c != null) return c;

        bool? b = CheckBool(value);
        if (b != null) return b;

        return value;
    }
}


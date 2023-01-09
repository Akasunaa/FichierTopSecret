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
    private static readonly string[] TruthyPropertyValues = { "true", "on", "yes", "enabled", "activated" };
    private static readonly string[] FalsyPropertyValues = { "false", "off", "no", "disabled", "deactivated" };

    public static readonly Dictionary<string, Func<string, object>> name2value = new Dictionary<string, Func<string, object>>()
    {
        { "position", CheckPosition },
        { "color", CheckColor },
        { "locked", CheckBool },
        { "power", CheckBool },
    };

    private static bool inSystemMessage;

    public static void VisualChange(string name, object value, GameObject go)
    {
        switch (name)
        {
            case "position":
                if (ModifiableController.TryParse(value, out Vector2Int pos))
                {
                    SetPosition(go, pos);
                }
                else
                {
                    Debug.LogWarning("Position badly written : " + value);
                }
                break;
            case "color":
                if (ModifiableController.TryParse(value, out Color color))
                {
                    SetColor(go, color);
                }
                else
                {
                    Debug.LogWarning("Color badly written : " + value);
                }
                break;
        }
    }

    private static void SetPosition(GameObject go, Vector2Int targetPosition)
    {
        Vector2 offset = Vector2.zero;
        Vector2? size = null;

        if (go.TryGetComponent(out BoxCollider2D collider))
        {
            offset = collider.offset * go.transform.lossyScale;
            size = collider.size * go.transform.lossyScale;
        }

        List<GameObject> hitGos =
            Utils.CheckPresencesOnTile(
                SceneData.Instance.grid,
                targetPosition + offset,
                size);


        bool shouldMove = true;

        // if the target tile is inocuppied or occupied by the go itself
        if (hitGos.Count > 0)
        {
            foreach (GameObject hitGo in hitGos)
            {
                // if one of the cllider is not the bject itself nor null
                if (hitGo != go && hitGo != null)
                {
                    shouldMove = false;
                    break;
                }
            }
        }

        if (shouldMove)
        {
            // move the object
            Vector3 targetPos = SceneData.Instance.grid.GetCellCenterWorld((Vector3Int)targetPosition);
            go.transform.position = targetPos;
            // update order in layer
            Utils.UpdateOrderInLayer(go);

            //since the object could be moved, we stop the error display if need be :
            if (inSystemMessage)
            {
                SceneData.Instance.dialogueUIController.EndDisplay();
                GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().RestartAllActions();
                inSystemMessage = false;
            }
        } else
        {
            string errorText = "I cannot move this object here, something is in the way! Better to move it elsewhere.";
            //Display a speech bubble indicating that the space is occupied and prevent player's interactions and movement during said time
            SceneData.Instance.dialogueUIController.DisplayDialogue(errorText, "player");
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopAllActions();
            inSystemMessage = true;
        }
        
    }

    private static void SetColor(GameObject go, Color color)
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
        else
        {
            SpriteRenderer[] sprites = go.GetComponentsInChildren<SpriteRenderer>(true);
            foreach(SpriteRenderer sprite in sprites)
            {
                sprite.color = color;
            }
        }
    }

    private static object CheckPosition(string value)
    {
        // pattern that we want into the value string - correct ex: (0,0) 
        const string number = @"(\-?)\d+";
        const string separator = @"[\ \;\,]+";

        if (!Regex.IsMatch(value, number + separator + number, options)) return null;

        // here we just want to extract the different decimals inside the value 
        var decodedCoordinates = Regex.Matches(value, number, options);

        float xTarget = float.Parse(decodedCoordinates[0].Value);
        float yTarget = float.Parse(decodedCoordinates[1].Value);

        return new Vector2Int((int)xTarget, (int)yTarget);
    }

    private static object CheckColor(string value)
    {
        // string[] colors = { "black", "blue", "cyan", "gray", "green", "grey", "magenta", "red", "white", "yellow" };
        // foreach (String colorValue in colors)
        // {
        //     if (Regex.IsMatch(value, colorValue, options))
        //     {
        //         value = colorValue;
        //         break;
        //     }
        // }
        if (ColorUtility.TryParseHtmlString(value, out Color color))
        {
            return color;
        }

        System.Drawing.Color c = System.Drawing.Color.FromName(value);
        if (c.IsKnownColor)
        {
            return new Color(c.R, c.G, c.B, c.A);
        }

        return null;
    }

    /**
     * Calculate the levenshtein distance between two words
     */
    private static int LevenshteinDistance(string subject, string model)
    {
        if (Mathf.Min(subject.Length, model.Length) == 0)
        {
            return Mathf.Max(subject.Length, model.Length);
        }
        else
        {
            string subSubject = subject.Substring(1);
            string subModel = model.Substring(1);

            if (subject[0] == model[0])
            {
                return LevenshteinDistance(subSubject, subModel);
            }
            else
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

        var closestPropertyName = PropertyNames[0];
        var levenshteinDistance = 10;

        foreach (var propertyName in PropertyNames)
        {
            var currentLevenshteinDistance = LevenshteinDistance(propertyNameInput, propertyName);
            if (currentLevenshteinDistance < levenshteinDistance)
            {
                closestPropertyName = propertyName;
                levenshteinDistance = currentLevenshteinDistance;
            }
        }

        if (levenshteinDistance <= inclusiveValidationThreshold)
        {
            return closestPropertyName;
        }
        else
        {
            return propertyNameInput;
        }
    }

    private static object CheckBool(string propertyValueInput)
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

    public static object ObjectFromValue(string name, string value)
    {
        if (name2value.TryGetValue(name, out Func<string, object> f))
        {
            object obj = f(value);
            if (obj != null) return obj;
        }

        return value;
    }
}

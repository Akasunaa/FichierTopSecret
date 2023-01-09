using System.Collections.Generic;
using UnityEngine;

/**
 *   Inherited class that will handle the modification of the files coming from the FileParser upon modification and/or other actions
 */
public abstract class ModifiableController : MonoBehaviour
{
    public bool canBeDeleted;

    public struct DicoValueProperty
    {
        public bool IsImportant;
        public object Value;
    }
    
    protected Dictionary<string, DicoValueProperty> properties { private set; get; } = new();
    
    public static bool TryParse<T>(object o, out T res)
    {
        if (o is T r)
        {
            res = r;
            return true;
        }

        res = default;
        return false;
    }

    public bool TryGet<T>(string key, out T res)
    {
        if (properties.TryGetValue(key, out DicoValueProperty o) && TryParse(o.Value, out T r))
        {
            res = r;
            return true;
        }

        res = default;
        return false;
    }

    /*
     * Check if properties contains the `key` and the value (associated with the key) has type T
     */
    public bool ContainsKey<T>(string key)
    {
        return TryGet(key, out T _);
    }

    public void SetValue(string key, object value)
    {
        var test = properties.TryGetValue(key, out var valued) && valued.IsImportant;
        if (!properties.TryAdd(key, new DicoValueProperty {IsImportant = test, Value = value}))
        {
            properties[key] = new DicoValueProperty {IsImportant = test, Value = value};
        }
    }

    public void RemoveValue(string key)
    {
        if (properties.ContainsKey(key))
        {
            properties.Remove(key);
        }
    }

    public abstract void SetDefaultProperties();

    /**
     *      Function called by the FileParser associated to the gameObject containing ModifiableController
     */
    public virtual void OnModification(string keyName, string value)
    {
        if (properties.TryGetValue(keyName, out var dicoValueProperty) && dicoValueProperty.Value.ToString() == value)
        {
            return;
        }

        print("Modifying " + keyName + " with value " + value + " from file");

        // fix typos and find a correct property
        var propertyName = ApplyPlayerChange.PropertyNameValidation(keyName);
        // return either "true" or "false" depending of the input string 
        // string propertyValue = ApplyPlayerChange.BooleanPropertyValueValidation(value);
        var objectValue = ApplyPlayerChange.ObjectFromValue(keyName, value);

        if (properties.ContainsKey(propertyName))
        {
            var wasImportant = properties[propertyName].IsImportant;
            properties[propertyName] = new DicoValueProperty {IsImportant = wasImportant, Value = objectValue};
        }
        else
        {
            // if there is a new property added we assume that it is not "important"
            properties.Add(propertyName, new DicoValueProperty {IsImportant = false, Value = objectValue});
        }
    }

    public virtual void UpdateModification()
    {
        foreach (var (keyName, dicoValueProperty) in properties)
        {
            ApplyPlayerChange.VisualChange(keyName, dicoValueProperty.Value, gameObject);
        }
    }

    public string ToFileString()
    {
        var col2Name = new Dictionary<Color, string>
        {
            { Color.white, "white" },
            { Color.black, "black" },
            { Color.blue, "blue" },
            { Color.green, "green" },
            { Color.red, "red" },
            { Color.yellow, "yellow" },
            { Color.grey, "grey" },
            { Color.cyan, "cyan" }
        };
        var res = "";
        foreach (var (keyName, dicoValueProperty) in properties)
        {
            switch (dicoValueProperty.Value)
            {
                case Color c:
                    if (col2Name.TryGetValue(c, out var colorName))
                    {
                        res += keyName + " : " + colorName + "\n";
                    }
                    else
                    {
                        res += keyName + " : #" + ColorUtility.ToHtmlStringRGB(c) + "\n";
                    }
                    break;
                case bool b:
                    var boolName = b ? "yes" : "no";
                    res += keyName + " : " + boolName + "\n";
                    break;
                default:
                    res += keyName + " : " + dicoValueProperty + "\n";
                    break;
            }
        }

        return res;
    }

    public void UpdateFile()
    {
        if (TryGetComponent(out FileParser fp))
        {
            fp.WriteToFile();
        }        
    }
}

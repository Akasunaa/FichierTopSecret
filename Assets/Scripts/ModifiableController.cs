using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

/**
 *   Inherited class that will handle the modification of the files coming from the FileParser upon modification and/or other actions
 */
public abstract class ModifiableController : MonoBehaviour
{
    public bool canBeDeleted;
    protected Dictionary<string, object> properties { private set; get; } = new Dictionary<string, object>();
    
    public static bool TryParse<T>(Object o, out T res)
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
        if (properties.TryGetValue(key, out object o) && TryParse(o, out T r))
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
        if (!properties.TryAdd(key, value))
        {
            properties[key] = value;
        }
    }

    public void RemoveValue(string key)
    {
        if (properties.ContainsKey(key))
        {
            properties.Remove(key);
        }
    }

    public virtual void SetDefaultProperties()
    {
        if (!properties.ContainsKey("position"))
        {
            Vector2Int pos = (Vector2Int)SceneData.Instance.grid.WorldToCell(transform.position);
            properties.TryAdd("position", pos);
        }
    }

    /**
     *      Function called by the FileParser associated to the gameObject containing ModifiableController
     */
    public virtual void OnModification(string keyName, string value)
    {
        if (properties.TryGetValue(keyName, out object obj) && obj.ToString() == value)
        {
            return;
        }

        print("Modifying " + keyName + " with value " + value + " from file");

        // fix typos and find a correct property
        string propertyName = ApplyPlayerChange.PropertyNameValidation(keyName);
        // return either "true" or "false" depending of the input string 
        // string propertyValue = ApplyPlayerChange.BooleanPropertyValueValidation(value);
        object objectValue = ApplyPlayerChange.ObjectFromValue(keyName, value);

        if (properties.ContainsKey(propertyName))
        {
            properties[propertyName] = objectValue;
        }
        else
        {
            properties.Add(propertyName, objectValue);
        }
    }

    public virtual void UpdateModification()
    {
        foreach ((string keyName, object value) in properties)
        {
            ApplyPlayerChange.VisualChange(keyName, value, gameObject);
        }
    }

    public string ToFileString()
    {
        Dictionary<Color, string> col2name = new Dictionary<Color, string>()
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
        string res = "";
        foreach ((string keyName, object value) in properties)
        {
            switch (value)
            {
                case Color c:
                    if (col2name.TryGetValue(c, out string colorName))
                    {
                        res += keyName + " : " + colorName + "\n";
                    }
                    else
                    {
                        res += keyName + " : #" + ColorUtility.ToHtmlStringRGB(c) + "\n";
                    }
                    break;
                case bool b:
                    string boolName = b ? "yes" : "no";
                    res += keyName + " : " + boolName + "\n";
                    break;
                default:
                    res += keyName + " : " + value + "\n";
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

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

    protected bool TryGet<T>(String key, out T res)
    {
        if (properties.TryGetValue(key, out Object o) && TryParse(o, out T r))
        {
            res = r;
            return true;
        }

        res = default;
        return false;
    }

    protected void SetValue(string key, object value)
    {
        if (!properties.TryAdd(key, value))
        {
            properties[key] = value;
        }
    }

    public abstract void SetDefaultProperties();

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
        object objectValue = ApplyPlayerChange.ObjectFromValue(value);
        
        Debug.Log("Test: " + keyName + " | " + objectValue.GetType());

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
        string res = "";
        foreach ((string keyName, object value) in properties)
        {
            switch (value)
            {
                case Color c:
                    res += keyName + " : #" + ColorUtility.ToHtmlStringRGB(c) + "\n";
                break;
                default:
                    res += keyName + " : " + value + "\n";
                    break;
            }
        }

        return res;
    }

    protected void UpdateFile()
    {
        if (TryGetComponent(out FileParser fp))
        {
            fp.WriteToFile();
        }        
    }
}

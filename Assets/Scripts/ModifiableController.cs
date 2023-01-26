using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using ColorUtility = UnityEngine.ColorUtility;

/**
 *   Inherited class that will handle the modification of the files coming from the FileParser upon modification and/or other actions
 */
public abstract class ModifiableController : MonoBehaviour
{
    public bool canBeDeleted;
    [HideInInspector] public UnityEvent<(string, object, object)> fileChange;

    protected struct DicoValueProperty
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

    public virtual void SetDefaultProperties()
    {
        if (!properties.ContainsKey("position"))
        {
            var pos = (Vector2Int)SceneData.Instance.grid.WorldToCell(transform.position);
            properties.TryAdd("position", new DicoValueProperty { IsImportant = true, Value = pos });
        }
    }

    /**
     *      Function called by the FileParser associated to the gameObject containing ModifiableController
     *      Return true if the properties has changed
     */
    public virtual bool OnModification(string keyName, string value)
    {
        // fix typos and find a correct property
        var propertyName = ApplyPlayerChange.PropertyNameValidation(keyName);
        // return either "true" or "false" depending of the input string 
        // string propertyValue = ApplyPlayerChange.BooleanPropertyValueValidation(value);
        var objectValue = ApplyPlayerChange.ObjectFromValue(propertyName, value);
        
        if (properties.TryGetValue(propertyName, out var dicoValueProperty) && dicoValueProperty.Value.ToString() == objectValue.ToString())
        {
            return false;
        }

        print("Modifying '" + keyName + "' with value '" + value + "' from file");

        if (properties.ContainsKey(propertyName))
        {
            fileChange.Invoke((keyName, objectValue, properties[propertyName].Value));
            var wasImportant = properties[propertyName].IsImportant;
            properties[propertyName] = new DicoValueProperty {IsImportant = wasImportant, Value = objectValue};
        }
        else
        {
            fileChange.Invoke((keyName, objectValue, null));
            // if there is a new property added we assume that it is not "important"
            properties.Add(propertyName, new DicoValueProperty {IsImportant = false, Value = objectValue});
        }

        return true;
    }

    public virtual void UpdateModification(bool firstRead = false)
    {
        string[] keys = properties.Keys.ToArray();
        foreach (string keyName in keys)
        {
            if (properties.ContainsKey(keyName))
            {
                switch (keyName)
                {
                    case "death":
                        if (TryGet("death", out bool death) && death)
                        {
                            SetValue("health", 0);
                        }
                        break;
                    case "power":
                        if (TryGet("power", out string power) && power.ToLower() == "unlimited")
                        {
                            SetValue("power", true);
                        }
                        break;
                    case "visible":
                        if (TryGet("visible", out bool visible) && !visible)
                        {
                            SetValue("color", new Color(1, 1, 1, 0.4f));
                        }
                        break;
                }
            }
        }
        keys = properties.Keys.ToArray();
        foreach (string keyName in keys)
        {
            if (properties.ContainsKey(keyName))
            {
                ApplyPlayerChange.VisualChange(keyName, properties[keyName].Value, gameObject);
            }
        }
    }

    public virtual string ToFileString()
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
                    res += keyName + " : " + dicoValueProperty.Value + "\n";
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
        else
        {
            Debug.LogWarning("No FileParser on this ModifiableController: " + name);
        }
    }

    /*
     * Return if the file should be re-written
     */
    public bool UpdatePropertiesDico(List<string> propertiesKeysList)
    {
        bool shouldRewriteFile = false;
        var keysToRemoveList = new List<string>();
        foreach (var (keyName, dicoValueProperty) in properties)
        {
            // search if the property key was updated
            var found = propertiesKeysList.Aggregate(false, (current, key) => ApplyPlayerChange.PropertyNameValidation(key.Trim().ToLower()) == keyName || current);

            // if it was not modified (is not in the file rn) && is not important, then remove it from the properties
            if (!found && !dicoValueProperty.IsImportant)
            {
                fileChange.Invoke((keyName, null, dicoValueProperty.Value));
                // properties.Remove(keyName); // does not work because it breaks the Iterator linked to properties in this foreach loop
                keysToRemoveList.Add(keyName);
            }
            else if (!found)
            {
                // We should re-write the file if an IsImportant property is deleted
                shouldRewriteFile = true;
            }
        }

        foreach (var key in keysToRemoveList)
        {
            properties.Remove(key);
        }

        return shouldRewriteFile;
    }

    /// <summary>
    /// Calls UpdateModification, UpdateFile and if the associated object has an ObjectInteractionController component,
    /// also calls ObjectInteractionController.OnChangeDialogue method.
    /// </summary>
    public void OnChangeUpdateAll()
    {
        UpdateModification();
        UpdateFile();

        var interactionController = GetComponent<ObjectInteractionController>();
        if (interactionController == null) return;
        interactionController.OnChangeDialogue();
    }
}

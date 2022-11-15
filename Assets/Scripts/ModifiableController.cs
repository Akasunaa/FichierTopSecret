using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FileParser))]

/**
 *   Inherited class that will handle the modification of the files coming from the FileParser upon modification and/or other actions
 */
public abstract class ModifiableController : MonoBehaviour
{
    public bool canBeDeleted;
    [SerializeField] protected Dictionary<string, string> properties = new Dictionary<string, string>(5);

    public abstract void setDefaultProperties();

    /**
     *      Function called by the FileParser associated to the gameObject containing ModifiableController
     */
    public virtual void OnModification(string name, string value)
    {
        print("Modifying " + name + " with value " + value + " from file");
        //MODIFICATION (inherited ?)

        // fix typos and find a correct property
        string propertyName = ApplyPlayerChange.PropertyNameValidation(name);
        // return either "true" or "false" depending of the input string 
        string propertyValue = ApplyPlayerChange.BooleanPropertyValueValidation(value);

        if (properties.ContainsKey(propertyName))
        {
            properties[propertyName] = propertyValue;
        }
        else
        {
            properties.Add(propertyName, propertyValue);
        }
    }

    public virtual void UpdateModification()
    {
        foreach ((string name, string value) in properties)
        {
            ApplyPlayerChange.VisualChange(name, value, gameObject);
        }
    }

    public string ToFileString()
    {
        string res = "";
        foreach ((string name, string value) in properties)
        {
            res += name + " : " + value + "\n";
        }

        return res;
    }

    public void UpdateFile()
    {
        GetComponent<FileParser>().WriteToFile();        
    }
}

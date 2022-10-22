using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *      Inherited class that will handle the modification of the files coming from the FileParser upon modification and/or other actions
 */
public abstract class ModifiableController : MonoBehaviour
{
    public bool canBeDeleted;
    [SerializeField] protected Dictionary<string, string> properties = new Dictionary<string, string>(5);

    /**
     *      Function called by the FileParser associated to the gameObject containing ModifiableController
     */
    public virtual void OnModification(string name, string value)
    {
        print("Modifying " + name + " with value " + value + " from file");
        //MODIFICATION (inherited ?)
        if (properties.ContainsKey(name))
        {
            properties[name] = value;
        }
        else
        {
            properties.Add(name, value);
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
        string res = "\n";
        foreach ((string name, string value) in properties)
        {
            res += name + " : " + value + "\n";
        }

        return res;
    }
}

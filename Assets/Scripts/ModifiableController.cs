using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *      Inherited class that will handle the modification of the files coming from the FileParser upon modification and/or other actions
 */
public class ModifiableController : MonoBehaviour
{
    public bool canBeDeleted;

    /**
     *      Function called by the FileParser associated to the gameObject containing ModifiableController
     */
    public virtual void OnModification(string variableModified, string value)
    {
        print("Modifying " + variableModified + " with value " + value + " from file");
        //MODIFICATION (inherited ?)
    }
}

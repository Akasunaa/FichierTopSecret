using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *      Inherited class that will handle the modification of the files coming from the FileLinkk upon modification
 */
public class ModifiableController : MonoBehaviour
{
    public void OnModification(string variableModified, string value)
    {
        print("Modifying " + variableModified + " with value " + value);
    }
}

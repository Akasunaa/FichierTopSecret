using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// Script used by the factory door, that will handle opening the door through the NPC interaction
/// </summary>
public class FactoryDoorController : DoorObjectController
{
    [Header("FactoryDoorElements")]
    [SerializeField] private string playerPrefsName;

    protected void Start()
    {
        if(PlayerPrefs.GetString(playerPrefsName) == "TRUE") //if the factory door has already been opened, it is unlocked at the scene's reloading
        {
            isLockedByDefault = false;
        }
    }

    /// <summary>
    /// Function called when the door is opened by the NPC factory guard
    /// </summary>
    public void UnlockDoor()
    {
        SetValue("locked", false);
        UpdateModification();
    }

}

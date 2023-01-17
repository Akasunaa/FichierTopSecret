using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryDoorController : DoorObjectController
{
    [Header("FactoryDoorElements")]
    [SerializeField] private string playerPrefsName;

    protected override void Awake()
    {
        base.Awake();
        if(PlayerPrefs.GetString(playerPrefsName) == "TRUE") //if the factory door has already been opened, it is unlocked at the scene's reloading
        {
            isLockedByDefault = false;
        }
    }

    /**
     *  Function called when the door is opened by the NPC factory guard
     */
    public void UnlockDoor()
    {
        Debug.Log("DOOR UNLOCKED");
        SetValue("locked", false);
        UpdateModification();
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This script will be used to create a special state only used by the factory guard that will open a door
/// </summary>
[CreateAssetMenu(fileName = "Factory Guard Special Dialog State", menuName = "States/Factory Guard Special Dialog State")]
public class FactoryGuardSpecialState : DialogState
{
    [Header("PlayerPrefs Elements")]
    [SerializeField] private string playerPrefsName;  //name of the player pref that we're going to modify upon call in the Enter() function
    [SerializeField] private string playerPrefsValue; //value inputted in said playerpref
    [Header("Factory Door Elements")]
    [SerializeField] private string factoryDoorTag="FactoryDoor";
    private GameObject factoryDoor;


    public FactoryGuardSpecialState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Function Enter() called when state is loaded.
    /// Main difference given by that script : 
    /// here, unlike regular dialog states, we're going to modify a player pref to a certain value AND also open the associated door
    /// </summary>
    /// <param name="sm">State Machine operating said state</param>
    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        factoryDoor = GameObject.FindGameObjectWithTag(factoryDoorTag);
        if (factoryDoor != null)
        {
            factoryDoor.GetComponent<FactoryDoorController>().UnlockDoor();
        }
        if (PlayerPrefs.GetString(playerPrefsName) != "READ") //if the value has already been read by the recepient, we do not change it anymore
        {
            PlayerPrefs.SetString(playerPrefsName, playerPrefsValue); //we write the wanted value
            PlayerPrefs.Save();                                       //we save it
        }
    }

}

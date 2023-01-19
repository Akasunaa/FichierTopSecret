using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This script will be used to create specialized dialog states that will, when entered, modify other .txt
/// To accomplish this, the script will modify values in the PlayerPrefs when its Enter() function inherited from StateMachine is called
/// </summary>
[CreateAssetMenu(fileName = "Player Prefs Dialog State", menuName = "States/Player Prefs Dialog State")]
public class PlayerPrefsDialogState : DialogState
{
    [Header("Special Dialog Elements")]
    [SerializeField] private string playerPrefsName;  //name of the player pref that we're going to modify upon call in the Enter() function
    [SerializeField] private string playerPrefsValue; //value inputted in said playerpref

    public PlayerPrefsDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Function Enter() called when state is loaded.
    /// Main difference given by that script : 
    /// here, unlike regular dialog states, we're going to modify a player pref to a certain value
    /// </summary>
    /// <param name="sm">State Machine operating the state</param>
    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        //Debug.Log("NPC SPECIAL DIALOG STATE : MODIFYING PLAYER PREFS");
        if (PlayerPrefs.GetString(playerPrefsName) != "READ") //if the value has already been read by the recepient, we do not change it anymore
        {
            PlayerPrefs.SetString(playerPrefsName, playerPrefsValue); //we write the wanted value
            PlayerPrefs.Save();                                       //we save it
            //Debug.Log("NPC SPECIAL DIALOG STATE : PLAYER PREF " + playerPrefsName + " WITH VALUE " + playerPrefsValue);
        }
    }


}

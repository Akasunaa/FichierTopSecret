using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *      This script will be used to create specialized dialog states that will, when entered, modify other .txt
 *      To accomplish this, the script will modify values in the PlayerPrefs when its Enter() function inherited from StateMachine is called
 */
[CreateAssetMenu(fileName = "Special Dialog State", menuName = "Special Dialog State")]
public class SpecialDialogState : DialogState
{
    [Header("Special Dialog Elements")]
    [SerializeField] private string playerPrefsName;  //name of the player pref that we're going to modify upon call in the Enter() function
    [SerializeField] private string playerPrefsValue; //value inputted in said playerpref

    public SpecialDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /**
     * Function Enter() called when state is loaded
     * main difference given by that script : 
     * here, unlike regular dialog states, we're going to modify a player pref to a certain value
    **/
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

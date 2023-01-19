using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// Script used by special dialog states that will, when ariving in their Enter() function, add a new property to the.txt of associated NPCs
/// </summary>
[CreateAssetMenu(fileName = "New Property Dialog State", menuName = "States/New Property Dialog State")]
public class NewPropertyDialogState : DialogState
{
    [Header("Properties to be added")]
    [SerializeField] private FILE_PROPERTIES[] fileProperties;

    public NewPropertyDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Function Enter() called when state is loaded.
    /// Main difference given by that script : 
    /// here, unlike regular dialog states, we're going to add the inputted fileProperties to the NPC's.txt
    /// </summary>
    /// <param name="sm">State Machine operating the state</param>
    public override void Enter(StateMachine sm)
    {
        base.Enter(sm);
        //Debug.Log("NON ACCESSIBLE DIALOG STATE : ENTER()");
        sm.ConvertTo<DialogSM>().associatedNPCController.AddProperty(fileProperties);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 *  Component used by objects that will allow the player to spew dialogue when interacting with them
 */
[RequireComponent(typeof(InteractableObjectController))]
public class ObjectInteractionController : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string dialogue;
    private DialogueUIController ui;                        //reference to the UI used for dialogs

    private void Start()
    {
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        Assert.IsNotNull(ui);
        if (dialogue == null)
        {
            dialogue = "ERROR : NO DIALOGUE WAS INPUTTED FOR THIS INTERACTION";
        }
    }

    /**
     *  Function called by ObjectControllers to display dialogue when interacting with them
     */
    public void DisplayInteractionDialogue()
    {
        ui.DisplayDialogue(dialogue, "player");
    }

    public void EndDisplay()
    {
        ui.EndDisplay();
    }
}

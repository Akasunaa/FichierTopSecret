using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Component used by objects that will allow the player to spew dialogue when interacting with them
/// </summary>
public class ObjectInteractionController : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string dialogueStandard;         //standard dialogue
    [TextArea(3, 10)]
    [SerializeField] private string dialogueOnConditionChange; //dialogue when change condition applied
    private string _curDialogue;
    private DialogueUIController _ui;                        //reference to the UI used for dialogs

    private void Start()
    {
        _ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        Assert.IsNotNull(_ui);
        dialogueStandard ??= "ERROR : NO DIALOGUE WAS INPUTTED FOR THIS INTERACTION";
        dialogueOnConditionChange ??= "WARNING : NO INPUTTED DIALOGUE FOR CHANGED CONDITION OF PREFAB. CHECK EDITOR.";
        _curDialogue = dialogueStandard;
    }

    /// <summary>
    /// Function called by ObjectControllers to display the currently loaded dialogue when interacting with them.
    /// </summary>
    public void DisplayInteractionDialogue()
    {
        _ui.DisplayDialogue(_curDialogue, "player");
    }

    /// <summary>
    /// Function handling the display of a specific piece of dialogue.
    /// </summary>
    /// <param name="dialogue">Dialogue that will be displayed.</param>
    public void DisplayDialogue(string dialogue) { _ui.DisplayDialogue(dialogue, "player"); }

    /// <summary>
    /// Function that will end the dialogue display.
    /// </summary>
    public void EndDisplay()
    {
        _ui.EndDisplay();
    }

    /// <summary>
    /// Function called by exterior scripts to change the currently displayed dialogue.
    /// </summary>
    public void OnChangeDialogue()
    {
        if (dialogueOnConditionChange == "") return;
        _curDialogue = _curDialogue == dialogueOnConditionChange ? dialogueStandard : dialogueOnConditionChange;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/**
 *  Script used to control the dialogue UI that appears when interacting with an object and/or NPC
 */
public class DialogueUIController : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Image backgroundDialogueImage;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void Awake()
    {
        Assert.IsNotNull(dialogueCanvas);
        Assert.IsNotNull(backgroundDialogueImage);
        Assert.IsNotNull(portrait);
        Assert.IsNotNull(dialogueText);
        dialogueCanvas.gameObject.SetActive(false);
    }

    /**
     *  Function called by external scripts when a dialogue is triggered and should be displayed
     *  Param :
     *      text : string : the displayed text needed to be displayed by the dialogue box
     */
    public void DisplayDialogue(string text)
    {
        dialogueCanvas.gameObject.SetActive(true);
        dialogueText.text = text;
        //WE SHOULD ALSO SET UP THE PORTRAIT OF THE ACTING NPC/PLAYER LINKED TO SAID DIALOGUE
    }

    /**
     * Function called when dialogue box should be quit after elements have been read
     */
    public void EndDisplay()
    {
        dialogueCanvas.gameObject.SetActive(false);
    }
}

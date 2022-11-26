using System;
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
    [Header("UI Elements")]
    [SerializeField] private Canvas dialogueCanvas;                 //canvas containing the dialogue items
    [SerializeField] private Image backgroundDialogueImage;         //probably not required
    [SerializeField] private Image portraitImage;                   //portrait image that will display the correct portrait sprite
    [SerializeField] private TextMeshProUGUI dialogueText;          //the dialogue text bubble

    [SerializeField] public PortraitObject[] availablePortraits;         //all the possible portraits in the game
    private Dictionary<string, Sprite> portraits;                   //all the possible portraits of the game

    private void Awake()
    {
        Assert.IsNotNull(dialogueCanvas);
        Assert.IsNotNull(backgroundDialogueImage);
        Assert.IsNotNull(portraitImage);
        Assert.IsNotNull(dialogueText);
        dialogueCanvas.gameObject.SetActive(false);
    }

    /**
     *  In Start function, we add to a dictionnary all the portraits and their associated indentifying strings
     */
    private void Start()
    {
        portraits = new Dictionary<string, Sprite>();
        foreach(var portrait in availablePortraits)
        {
            portraits.Add(portrait.portraitName, portrait.portraitSprite);
        }
        print(portraits);
    }

    /**
     *  Function called by external scripts when a dialogue is triggered and should be displayed
     *  Param :
     *      text : string : the displayed text needed to be displayed by the dialogue box
     *      name : string : the entity saying the line (player or npc)
     */
    public void DisplayDialogue(string text, string name)
    {
        dialogueCanvas.gameObject.SetActive(true);
        dialogueText.text = text;
        if (portraits.TryGetValue(name, out Sprite curPortrait)) //we obtain the current portrait based on the inputted name
        {
            portraitImage.sprite = curPortrait;
        }
        else
        {
            portraitImage.sprite = null;
        }
    }

    /**
     * Function called when dialogue box should be quit after elements have been read
     */
    public void EndDisplay()
    {
        dialogueCanvas.gameObject.SetActive(false);
    }

    public bool ContainsPortrait(string nameCheck)
    {
        foreach(var portrait in availablePortraits)
        {
            if (portrait.portraitName == nameCheck)
            {
                return true;
            }
        }
        return false;
    }
}

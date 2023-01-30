using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Script used to control the dialogue UI that appears when interacting with an object and/or NPC
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Canvas dialogueCanvas;                 //canvas containing the dialogue items
    [SerializeField] private UnityEngine.UI.Image portraitImage;                   //portrait image that will display the correct portrait sprite
    [SerializeField] private TextMeshProUGUI dialogueText;          //the dialogue text bubble
    [SerializeField] private TextMeshProUGUI dialogNameText;        //name displayed during the dialog
    public GameObject cinematicCanvas;

    [SerializeField] public PortraitObject[] availablePortraits;         //all the possible portraits in the game
    private Dictionary<string, Sprite> portraits;                   //all the possible portraits of the game

    private void Awake()
    {
        Assert.IsNotNull(dialogueCanvas);
        Assert.IsNotNull(portraitImage);
        Assert.IsNotNull(dialogueText);
        dialogueCanvas.gameObject.SetActive(false);
        // we add to a dictionnary all the portraits and their associated indentifying strings
        portraits = new Dictionary<string, Sprite>();
        foreach (var portrait in availablePortraits) 
        {
            portraits.Add(portrait.portraitName, portrait.portraitSprite);
        }
    }

    /**
     *  In Start function, we add to a dictionnary all the portraits and their associated indentifying strings
     */
    private void Start()
    {
        //portraits = new Dictionary<string, Sprite>();
        //foreach(var portrait in availablePortraits)
        //{
        //    portraits.Add(portrait.portraitName, portrait.portraitSprite);
        //}
    }

    /// <summary>
    /// Function called by external scripts when a dialogue is triggered and should be displayed
    /// </summary>
    /// <param name="text">the displayed text needed to be displayed by the dialogue box</param>
    /// <param name="name">name of the entity saying the line that references a name in the portrait boxes</param>
    public void DisplayDialogue(string text, string name)
    {
        dialogueCanvas.gameObject.SetActive(true);
        dialogueText.text = text;
        if (portraits.TryGetValue(name, out Sprite curPortrait)) //we obtain the current portrait based on the inputted name
        {
            portraitImage.sprite = curPortrait;
            dialogNameText.text = name;
        }
        else
        {
            dialogNameText.text = name;
            portraitImage.sprite = null;
        }
    }

    /// <summary>
    /// Function called when dialogue box should be quit after elements have been read
    /// </summary>
    public void EndDisplay()
    {
        dialogueCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Function that will return wether the inputted string refers to a valid portrait
    /// </summary>
    /// <param name="nameCheck">name of portrait being checked for</param>
    /// <returns>true if said name is found in the portrait list, false otherwise</returns>
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

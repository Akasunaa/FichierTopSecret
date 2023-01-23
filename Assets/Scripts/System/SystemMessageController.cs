using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to display messages during playtime, as self-reflexions by the player of errors/things happening.
/// Singleton pattern.
/// </summary>
public class SystemMessageController : MonoBehaviour
{
    private DialogueUIController ui;
    private bool isDisplayindDialogue;

    #region SINGLETON PATTERN
    private static SystemMessageController _instance;
    public static SystemMessageController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SystemMessageController();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>();
    }

    private void Update()
    {
        if(isDisplayindDialogue && Input.GetKeyDown(KeyCode.E)) 
        {
            isDisplayindDialogue = false;
            ui.EndDisplay();
        }
    }

    /// <summary>
    /// Function that handles the display of system messages.
    /// </summary>
    /// <param name="message">Message to be displayed</param>
    /// <param name="portraitRef">Reference of the portrait for the displayed dialogue.</param>
    public void CallSystemMessage(string message, string? portraitRef)
    {
        if(!isDisplayindDialogue)
        {
            portraitRef = portraitRef==null ? "player" : portraitRef;
            ui.DisplayDialogue(message, portraitRef);
        }
    }
}

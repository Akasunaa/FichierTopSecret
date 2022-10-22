using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 *  DEBUG ONLY : ALLOWS TO TEST THE DIALOG STATE MACHINE
 */
public class TEST : MonoBehaviour
{
    public bool startInteraction;
    private bool interactionStarted;
    public bool Interact;
    public bool ChangeState;
    public int newStateIndex;
    public string curDisplayPortrait;
    [SerializeField] private GameObject NPC;

    // Start is called before the first frame update
    void Start()
    {
        Interact = false;
        ChangeState = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startInteraction && !interactionStarted)
        {
            StartInteraction();
            startInteraction = false;
        }
        if (Interact == true && interactionStarted)
        {
            NPC.GetComponent<DialogSM>().OnDialogInteraction();
            GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>().DisplayDialogue(NPC.GetComponent<DialogSM>().currentState.ConvertTo<DialogState>().currentSpeech, curDisplayPortrait);
            Interact = false;
        }
        if(ChangeState == true && interactionStarted)
        {
            NPC.GetComponent<DialogSM>().ChangeState(newStateIndex);
            GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>().EndDisplay();
            interactionStarted = false;
            Time.timeScale = 1f; //restore player movement
            ChangeState = false;
        }
    }

    private void StartInteraction()
    {
        interactionStarted = true;
        Time.timeScale = 0f; //prevent player movement
        GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>().DisplayDialogue(NPC.GetComponent<DialogSM>().currentState.ConvertTo<DialogState>().currentSpeech, curDisplayPortrait);
    }
}

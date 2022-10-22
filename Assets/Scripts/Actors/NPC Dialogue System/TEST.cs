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
            GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>().DisplayDialogue(NPC.GetComponent<DialogSM>().currentState.ConvertTo<DialogState>().currentSpeech);
            Interact = false;
        }
        if(ChangeState == true && interactionStarted)
        {
            NPC.GetComponent<DialogSM>().ChangeState(newStateIndex);
            GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>().EndDisplay();
            interactionStarted = false;
            ChangeState = false;
        }
    }

    private void StartInteraction()
    {
        interactionStarted = true;
        GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>().DisplayDialogue(NPC.GetComponent<DialogSM>().currentState.ConvertTo<DialogState>().currentSpeech);
    }
}

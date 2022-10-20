using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 *  DEBUG ONLY : ALLOWS TO TEST THE DIALOG STATE MACHINE
 */
public class TEST : MonoBehaviour
{
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
        if (Interact == true)
        {
            NPC.GetComponent<DialogSM>().OnDialogInteraction();
            Interact = false;
        }
        if(ChangeState == true)
        {
            NPC.GetComponent<DialogSM>().ChangeState(newStateIndex);
            ChangeState = false;
        }
    }
}

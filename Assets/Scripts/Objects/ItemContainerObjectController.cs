using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;

/**
 *  Component used by Item-containers
 *  These containers can be either locked or not and will contain an item
 *  When interacting with them, the player will gain said items
 */
public class ItemContainerObjectController : ModifiableController, Interactable
{
    [SerializeField] private GameObject item; //Item that the desk contains
    private DialogueUIController ui;                        //reference to the UI used for dialogs
    private bool hasItem = true; //boolean that will remove the container giving infinite items
    protected bool isInInteraction;

    //The various dialogues that can be displayed :
    [TextArea(3, 10)]
    [SerializeField] private string dialogueLocked;
    [TextArea(3, 10)]
    [SerializeField] private string dialogueAlreadyTakenItem;
    [TextArea(3, 10)]
    [SerializeField] private string dialogueItemRecuperate;

    public bool canBeInteracted { get; set; }

    private void Start()
    {
        isInInteraction = false;
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        Assert.IsNotNull(ui);
    }

    public override void SetDefaultProperties()
    {
        properties.Add("locked", true);
    }

    public void Interact()
    {
        if (!isInInteraction)
        {
            isInInteraction = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();
        }
        else
        {
            ui.EndDisplay();
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().ClearMovement();
            return;
        }
        //if (Time.timeScale == 0f)
        //{
        //    ui.EndDisplay();
        //    Time.timeScale = 1f;
        //    return;
        //}
        //else
        //{
        //    Time.timeScale = 0f;    //if player in interaction, then stop time to prevent movement
        //}
        if (TryGet("locked", out bool locked))
        {
            if (!locked && hasItem)
            {
                RecuperateItem();
                ui.DisplayDialogue(dialogueItemRecuperate, "player");
            }
            else if (!hasItem)
            {
                ui.DisplayDialogue(dialogueAlreadyTakenItem, "player");
            }
            else
            {
                ui.DisplayDialogue(dialogueLocked, "player");
            }

            UpdateModification();
            UpdateFile();
        }
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For Item Container, we test if player can open it or not (visual change)
        if (TryGet("locked", out bool locked))
        {
            if (locked)
            {
                //CHANGE VISUALLY
                Debug.Log("ITEM CONTAINER : LOCKED");
            }
            else
            {
                //CHANGE VISUALLY
                Debug.Log("ITEM CONTAINER : NOT LOCKED");
            }
        }
    }

    /**
     *  Function called when player interacts with ItemContainer when it is not locked
     *  It will create an instance of the stored item, and call its internal ItemController.RecuperatingItem() function
     */
    private void RecuperateItem()
    {
        hasItem = false;
        GameObject new_item = Instantiate(item);
        new_item.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
        new_item.GetComponent<ItemController>().RecuperatingItem();
    }
}

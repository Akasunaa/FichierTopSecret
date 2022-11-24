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

    public bool canBeInteracted { get; set; }

    private void Start()
    {
        ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        Assert.IsNotNull(ui);
    }

    public override void SetDefaultProperties()
    {
        properties.Add("locked", true);
    }

    public void Interact()
    {
        if (Time.timeScale == 0f)
        {
            ui.EndDisplay();
            Time.timeScale = 1f;
            return;
        }
        else
        {
            Time.timeScale = 0f;    //if player in interaction, then stop time to prevent movement
        }
        if (TryGet("locked", out bool locked))
        {
            if (!locked && hasItem)
            {
                RecuperateItem();
                ui.DisplayDialogue("Looks like I found an item !", "player");
            }
            else if (!hasItem)
            {
                ui.DisplayDialogue("Seems like I have already taken that item...", "player");
            }
            else
            {
                ui.DisplayDialogue("Looks like it's locked ! I need to find a way to unlock it...", "player");
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

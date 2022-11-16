using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/**
 *  Component used by Item-containers
 *  These containers can be either locked or not and will contain an item
 *  When interacting with them, the player will gain said items
 */
public class ItemContainerObjectController : ModifiableController, Interactable
{
    [SerializeField] private GameObject item; //Item that the desk contains
    public bool canBeInteracted { get; set; }

    public override void setDefaultProperties()
    {
        properties.Add("locked", "true");
    }

    public void Interact()
    {
        if (properties.ContainsKey("locked"))
        {
            if (properties["locked"] == "false")
            {
                RecuperateItem();
                Debug.Log("ITEM CONTAINER : RECUPERATED ITEM");
            }
            else
            {
                Debug.Log("ITEM CONTAINER : ITEM CONTAINER LOCKED");
            }

            UpdateModification();
            UpdateFile();
        }
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For Item Container, we test if player can open it or not (visual change)
        if (properties.ContainsKey("status"))
        {
            if (properties["locked"] == "true")
            {
                //CHANGE VISUALLY
                Debug.Log("ITEM CONTAINER : LOCKED");
            }
            else if (properties["locked"] == "false")
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
        GameObject new_item = Instantiate(item);
        new_item.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
        new_item.GetComponent<ItemController>().RecuperatingItem();
    }
}

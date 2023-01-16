using UnityEngine;
using UnityEngine.Assertions;

/**
 *  Component used by Item-containers
 *  These containers can be either locked or not and will contain an item
 *  When interacting with them, the player will gain said items
 */
public class ItemContainerObjectController : ModifiableController, Interactable
{
    [SerializeField] private GameObject item; //Item that the desk contains
    private DialogueUIController _ui;                        //reference to the UI used for dialogs
    private bool _hasItem = true; //boolean that will remove the container giving infinite items
    protected bool IsInInteraction;

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
        IsInInteraction = false;
        _ui = GameObject.FindGameObjectsWithTag("UI")[0].GetComponent<DialogueUIController>();
        Assert.IsNotNull(_ui);
    }

    public override void SetDefaultProperties()
    {
        properties.TryAdd("locked", new DicoValueProperty {IsImportant = true, Value = true});
    }

    public void Interact()
    {
        Debug.Log("INTERACTION TRIGGERED IN ITEM CONTAINER WITH STATUS : "+IsInInteraction);
        if (!IsInInteraction)
        {
            IsInInteraction = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();
        }
        else
        {
            _ui.EndDisplay();
            IsInInteraction = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().RestartMovement();
            return;
        }
        if (TryGet("locked", out bool locked))
        {
            if (!locked && _hasItem)
            {
                //Debug.Log("INTERACTION : RECUPERATING ITEM");
                RecuperateItem();
                _ui.DisplayDialogue(dialogueItemRecuperate, "player");
            }
            else if (!_hasItem)
            {
                if (!CheckItemPresence()) //if the player doesn't have the item BUT it was already taken, we give it one more time
                {
                    //Debug.Log("INTERACTION : RECUPERATING AGAIN ITEM");
                    RecuperateItem();
                    _ui.DisplayDialogue(dialogueItemRecuperate, "player");
                }
                else
                {
                    //Debug.Log("INTERACTION : NO ITEM");
                    _ui.DisplayDialogue(dialogueAlreadyTakenItem, "player");
                }
            }
            else
            {
                //Debug.Log("INTERACTION : LOCKED");
                _ui.DisplayDialogue(dialogueLocked, "player");
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
        _hasItem = false;
        LevelManager.GiveItem(item);
        //GameObject new_item = Instantiate(item);
        //new_item.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
        //new_item.GetComponent<ItemController>().RecuperatingItem();
    }

    /**
     *  Function that will test, using a virtual item instantly destroyed, if said item exists or not
     */
    private bool CheckItemPresence()
    {
        GameObject new_item = Instantiate(item);
        if (new_item.GetComponent<ItemController>().CheckPresenceItem()) //if player has item
        {
            Destroy(new_item);
            return true;
        }
        else
        {
            Destroy(new_item);
            return false;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.VisualScripting;

public class InputController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteractionController interactionController;
    private Inputs input;
    private Inputs.OnFootActions onFoot;
    private int nbKeyPressed;

    private bool inInteraction; //boolean indicating if speech is currently being displayed -> prevents player movements (allows interaction)
    private bool inSystemMessage; //boolean indicating if system message is currently displayed -> prevents all player actions

    private void Awake()
    {
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
        if (!interactionController) interactionController = GetComponent<PlayerInteractionController>();
        input = new Inputs();
        onFoot = input.onFoot;
        nbKeyPressed = 0;
        inInteraction = false;
        inSystemMessage = false;
    }

    /**
     * Called on each movement input
     */
    public void Move(InputAction.CallbackContext context)
    {
        if (!inInteraction)
        {
            PerformInput(context);
            ResetInput(context);
        }
    }

    /**
     * action realised when pressing a key
     * increment the number of keys pressed and adds the corresponding vector on top of the input pile
     */
    private void PerformInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // increase the number of keys pressed
            nbKeyPressed++;

            playerMovement.AddMovementInStack(InputVectorConversion(context.action.activeControl));
        }
    }

    /**
     * action realised when releasing a key
     * decrement the number of keys pressed and removes the corresponding vector in the input pile
     */
    private void ResetInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            // decrease the number of keys pressed,reset movement only if all the keys are released
            nbKeyPressed--;
            if (nbKeyPressed < 0) { nbKeyPressed = 0; }

            playerMovement.RemoveMovementInStack(InputVectorConversion(context.action.activeControl));
            if (nbKeyPressed == 0)
            {
                // we clear the input pile if no keys are pressed anymore for safe measure
                playerMovement.ClearInputStack();
            }
        }
    }

    /**
     * Convert the InputControl that the context gives us into its vector equivalent
     */
    private Vector2Int InputVectorConversion(InputControl inputControl)
    {
        string inputName = inputControl.name;
        if (inputName == "w")
        {
            inputName = "z";
        }
        if (inputName == "a")
        {
            inputName = "q";
        }


        if (inputName.Equals(input.onFoot.HorizontalLeftMovement.GetBindingDisplayString(1).ToLower()) || inputName == "leftArrow")
        {
            return Vector2Int.left;
        }
        if (inputName.Equals(input.onFoot.VerticalUpMovement.GetBindingDisplayString(1).ToLower()) || inputName == "upArrow")
        {
            return Vector2Int.up;
        }
        if (inputName.Equals(input.onFoot.VerticalDownMovement.GetBindingDisplayString(1).ToLower()) || inputName == "downArrow" )
        {
            return Vector2Int.down;
        }
        if (inputName.Equals(input.onFoot.HorizontalRightMovement.GetBindingDisplayString(1).ToLower()) || inputName == "rightArrow")
        {
            return Vector2Int.right;
        }
        return Vector2Int.zero;
    } 

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && !inSystemMessage)
        {
            PlayerInteractionController pic = GetComponent<PlayerInteractionController>();
            if (pic.lastInteractable!=null && pic.lastInteractable.canBeInteracted)
            {
                pic.lastInteractable.Interact();
            }
        }
    }

    /**
     *  Function called by objects when they need to stop the player's movement
     */
    public void StopMovement()
    {
        playerMovement.ClearInputStack();
        inInteraction = true;
    }

    /**
     *  Function called by objects when they need to reactivate the player's movement
     */
    public void RestartMovement()
    {
        playerMovement.ClearInputStack();
        inInteraction=false;
        //onInteraction.Disable();
        //onFoot.Enable();
        //Debug.Log("INPUTS : onFoot : " + onFoot.enabled);
        //Debug.Log("INPUTS : onInteraction : " + onInteraction.enabled);
    }

    /**
     *  Killswitch taking all controls away from the player
     */
    public void StopAllActions()
    {
        playerMovement.ClearInputStack();
        inInteraction = true;
        inSystemMessage = true;
    }

    /**
    *  Killswitch giving back all the controls from the player
    */
    public void RestartAllActions()
    {
        playerMovement.ClearInputStack();
        inInteraction = false;
        inSystemMessage = false;
    }

    private void OnEnable()
    {
        onFoot.Enable();

    }

    private void OnDestroy()
    {
        onFoot.Disable();
    }
}

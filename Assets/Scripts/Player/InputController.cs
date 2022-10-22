using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InteractionController interactionController;
    private Inputs input;
    private Inputs.OnFootActions onFoot;
    private int nbKeyPressed;

    private void Awake()
    {
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
        if (!interactionController) interactionController = GetComponent<InteractionController>();
        input = new Inputs();
        onFoot = input.onFoot;
        nbKeyPressed = 0;
    }

    /**
     * Called on each movement input
     */
    public void Move(InputAction.CallbackContext context)
    {
        PerformInput(context);
        ResetInput(context);
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
        Debug.Log("^canceled");
            // decrease the number of keys pressed,reset movement only if all the keys are released
            nbKeyPressed--;

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

        if (inputName.Equals(input.onFoot.HorizontalLeftMovement.GetBindingDisplayString(1).ToLower()))
        {
            return Vector2Int.left;
        }
        if (inputName.Equals(input.onFoot.VerticalUpMovement.GetBindingDisplayString(1).ToLower()))
        {
            return Vector2Int.up;
        }
        if (inputName.Equals(input.onFoot.VerticalDownMovement.GetBindingDisplayString(1).ToLower()))
        {
            return Vector2Int.down;
        }
        if (inputName.Equals(input.onFoot.HorizontalRightMovement.GetBindingDisplayString(1).ToLower()))
        {
            return Vector2Int.right;
        }
        return Vector2Int.zero;
    } 

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            interactionController.InteractionOrder();
        }
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
}

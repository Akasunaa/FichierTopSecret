using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    private Inputs input;
    private Inputs.OnFootActions onFoot;
    private int nbKeyPressed;

    private void Awake()
    {
        input = new Inputs();
        onFoot = input.onFoot;
        nbKeyPressed = 0;
    }

    public void MoveVertically(InputAction.CallbackContext context)
    {
        PerformInput(context, Vector2Int.up);
        ResetInput(context);
    }

    public void MoveHorizontally(InputAction.CallbackContext context)
    {
        PerformInput(context, Vector2Int.right);
        ResetInput(context);
    }

    private void PerformInput(InputAction.CallbackContext context, Vector2Int direction)
    {
        if (context.performed)
        {
            // decrease the number of keys pressed,reset movement only if all the keys are released
            nbKeyPressed++;
            float value = context.ReadValue<float>();
            playerMovement.SetDirection((int) value * direction);
        }
    }

    private void ResetInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            // decrease the number of keys pressed,reset movement only if all the keys are released
            nbKeyPressed--;
            if (nbKeyPressed == 0)
            {
                playerMovement.SetDirection(Vector2Int.zero);
            }
        }
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
}

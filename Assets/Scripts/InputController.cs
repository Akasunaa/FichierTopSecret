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
        if (context.performed)
        {
            nbKeyPressed++;
            float value = context.ReadValue<float>();
            playerMovement.SetDirection(new Vector2Int(0, (int)value));
        }

        if (context.canceled)
        {
            // decrease the number of keys pressed, reset movement only if all the keys are released
            nbKeyPressed--;
            if (nbKeyPressed == 0)
            {
                playerMovement.SetDirection(Vector2Int.zero);
            }
        }
    }

    public void MoveHorizontally(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            nbKeyPressed++;
            float value = context.ReadValue<float>();
            playerMovement.SetDirection(new Vector2Int((int)value, 0));
        }

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

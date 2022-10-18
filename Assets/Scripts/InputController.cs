using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;

    private Inputs input;
    private Inputs.OnFootActions onFoot;

    private void Awake()
    {
        input = new Inputs();
        onFoot = input.onFoot;
    }

    private void FixedUpdate()
    {
        Movement(onFoot.Movement.ReadValue<Vector2>());
    }

    private void Movement(Vector2 input)
    {
        playerMovement.MovementOrder(new Vector2Int((int)input.x, (int)input.y));
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
}

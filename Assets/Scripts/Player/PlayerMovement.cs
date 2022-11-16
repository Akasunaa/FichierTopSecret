using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;


public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Grid grid;                                     // grid guiding the tilemaps
    [SerializeField] private Animator animator;                             // player animations 
    private PlayerInteractionController interactionController;              //for collision with object in scene

    [Header("Movement variables")]
    [SerializeField] private float speed;
    
    [Header("Debug")]
    [SerializeField] private Vector2Int tilemapPosition;                    // position of the player on the tilemap
    [SerializeField] private List<Vector2Int> inputStack;                   // stack of inputs from least to most recent
    public Vector2Int facingDirection { get; private set; }                 // vector indicating in which direction the player is facing  
    private bool isMoving;


    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        interactionController = GetComponent<PlayerInteractionController>();

        facingDirection = Vector2Int.zero;
        inputStack = new List<Vector2Int>();
        isMoving = false;

        animator.speed = speed;
    }

    private void Start()
    {
        grid = SceneData.Instance.grid;

        // set position exactly on a tile
        tilemapPosition = (Vector2Int)grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld((Vector3Int)tilemapPosition);
    }

    private void FixedUpdate()
    {   
        if (!isMoving)
        {
          Move();
        }
    }

    private void Move()
    {
        if (inputStack.Count > 0)
        {
            // set direction as the top of the input stack
            facingDirection = inputStack[^1];
            // calculate target position
            Vector3Int targetTilemapPosition = grid.WorldToCell(transform.position) + (Vector3Int)facingDirection;

            // refresh the info containing which side the character is facing
            RefreshOrientationSprite(facingDirection);

            // check if the cell is occupied
            if (interactionController.IsColliding(targetTilemapPosition))
            {
                return;
            }

            // start the movement
            Debug.DrawRay(grid.GetCellCenterWorld(targetTilemapPosition), Vector2.up/100 , Color.green, 10);
            StartCoroutine(SmoothMovement(targetTilemapPosition));
            tilemapPosition = (Vector2Int) grid.WorldToCell(transform.position);
        }
    }

    private IEnumerator SmoothMovement(Vector3Int targetPosition)
    {
        isMoving = true;

        // keep initial position
        Vector3 initialPosition = transform.position;

        // get animation clip information 
        animator.SetTrigger("WalkTrigger");
        
        yield return new WaitForSeconds(0.001f);
        float movementCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed;
       
        float timer = 0;
        while (timer < movementCooldown)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, grid.GetCellCenterWorld(targetPosition), timer / movementCooldown) ;
            yield return null;
        }

        isMoving = false;
        //Check interaction
        interactionController.CheckForInteraction(transform.position,facingDirection);
    }

    private void RefreshOrientationSprite(Vector2Int direction)
    {
        animator.SetFloat("dirX", direction.x);
        animator.SetFloat("dirY", direction.y);
        //Check interaction
        interactionController.CheckForInteraction(transform.position,facingDirection);
    }

    public void AddMovementInStack(Vector2Int dir)
    {   
        inputStack.Add(dir);
    }

    /**
     * find the right vector in the input stack and removes it
     */
    public void RemoveMovementInStack(Vector2Int dir)
    {
        inputStack.Remove(dir);
    }

    public void ClearInputStack()
    {
        inputStack.Clear();
    }

    public Vector2Int GetTilemapPosition()
    {
        return tilemapPosition;
    }

    public Vector2Int GetOrientation()
    {
        return facingDirection;
    }
}
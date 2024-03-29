using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using UnityEngine.Events;


public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Grid grid; //todo : search the grid in SceneData // grid guiding the tilemaps
    [SerializeField] private Animator animator;                             // player animations 
    private PlayerInteractionController interactionController;              //for collision with object in scene

    [Header("Movement variables")]
    [SerializeField] private float speed;

    [SerializeField] public Vector2Int tilemapPosition { get; private set; } // position of the player on the tilemap
    [SerializeField] private List<Vector2Int> inputStack;                   // stack of inputs from least to most recent

    public UnityEvent onMovementFinish;
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
            RefreshOrientationSprite();

            // check if the cell is occupied
            if (Utils.CheckPresencesOnTile(grid, targetTilemapPosition).Count > 0)
            {
                return;
            }

            // start the movement
            //Debug.DrawRay(grid.GetCellCenterWorld(targetTilemapPosition), Vector2.up/100 , Color.green, 10);
            StartCoroutine(SmoothMovement(targetTilemapPosition));
            tilemapPosition = (Vector2Int)grid.WorldToCell(transform.position);
        }
    }

    private IEnumerator SmoothMovement(Vector3Int targetPosition)
    {
        isMoving = true;
        bool hasUpdatedOrderInLayer = false;

        // keep initial position
        Vector3 initialPosition = transform.position;

        // get animation clip information 
        animator.SetTrigger("WalkTrigger");

        animator.Update(0.001f);
        float movementCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed;

        float timer = 0;
        while (timer < movementCooldown)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, grid.GetCellCenterWorld(targetPosition), timer / movementCooldown) ;

            // Update Order in layer in the middle of the movements
            if (timer >= movementCooldown/2 && !hasUpdatedOrderInLayer)
            {
                Utils.UpdateOrderInLayer(gameObject);
                hasUpdatedOrderInLayer = true;
            }
            
            yield return null;
        }

        isMoving = false;

        // Check interaction
        interactionController.CheckForInteraction(transform.position,facingDirection);

        // invoke an event at the end of the movement
        onMovementFinish.Invoke();
    }

    public void RefreshOrientationSprite()
    {
        animator.SetFloat("dirX", facingDirection.x);
        animator.SetFloat("dirY", facingDirection.y);
        //Check interaction
        interactionController.CheckForInteraction(transform.position, facingDirection);
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

    public Animator GetAnimator()
    {
        return animator;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
        animator.speed = speed;
    }
}

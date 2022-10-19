using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;


public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Grid grid;                                     // grid guiding the tilemaps
    [SerializeField] private Tilemap[] obstacleTilemaps;                    // array with all the tilemaps with which the player should collide
    [SerializeField] private Animator animator;                             // player animations 

    [Header("Movement variables")]
    [SerializeField] private Vector2Int tilemapPosition;                    // position of the player on the tilemap
    private int walkFrameCooldown;                         // cooldown between to tile displacement, should be equal to the walk animation duration
    [SerializeField] private List<Vector2Int> inputPile;                    // pile of inputs from least to most recent

    private bool isMoving;
    private Vector2Int facingDirection;                                     // vector indicating in which direction the player is facing  

    private void Awake()
    {
        Assert.IsNotNull(grid);
        if (!animator) animator = GetComponentInChildren<Animator>();

        facingDirection = Vector2Int.zero;
        inputPile = new List<Vector2Int>();
        isMoving = false;

        // set position exactly on a tile
        tilemapPosition = (Vector2Int) grid.WorldToCell(transform.position);
        transform.position = grid.CellToWorld((Vector3Int) tilemapPosition);
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
        if (inputPile.Count > 0)
        {
            // set direction as the top of the input pile
            facingDirection = inputPile[inputPile.Count - 1];
            // calculate target position
            Vector3Int targetTilemapPosition = grid.WorldToCell(transform.position) + (Vector3Int)facingDirection;

            // check if the cell is occupied
            if (obstacleTilemaps != null)
            {
                foreach(Tilemap tilemap in obstacleTilemaps)
                {
                    if (tilemap.HasTile(targetTilemapPosition))
                    {
                        return;
                    }
                }
            }


            
            
            // start the movement
            StartCoroutine(SmoothMovement(targetTilemapPosition));
            tilemapPosition = (Vector2Int) grid.WorldToCell(transform.position);
        }
    }

    private IEnumerator SmoothMovement(Vector3Int targetPosition)
    {
        isMoving = true;

        // refresh the info containing which side the character is facing
        RefreshOrientationSprite(facingDirection);

        // keep initial position
        Vector3 initialPosition = transform.position;

        // get animation clip information 
        animator.SetTrigger("WalkTrigger");
        float walkFrameCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;

        int frameCounter = 0;
        while (frameCounter < walkFrameCooldown)
        {
            frameCounter++;

            //transform.position = Vector3.Lerp(initialPosition, grid.CellToWorld(targetPosition), (float) frameCounter / walkFrameCooldown) + new Vector3(grid.cellSize.x / 2, 0, 0);
            transform.position = Vector3.Lerp(initialPosition, grid.CellToWorld(targetPosition), (float) frameCounter / walkFrameCooldown);

            yield return null;
        }

        isMoving = false;
    }

    private void RefreshOrientationSprite(Vector2Int direction)
    {

        animator.SetFloat("X", direction.x);
        animator.SetFloat("Y", direction.y);
    }


    public void AddMovementInPile(Vector2Int dir)
    {
        inputPile.Add(dir);
        RefreshOrientationSprite(dir);
    }

    /**
     * find the right vector in the input pile and removes it
     */
    public void RemoveMovementInPile(Vector2Int dir)
    {
        if (inputPile.Count > 0)
        {
            for (int i = 0; i < inputPile.Count; i++)
            {
                Vector2Int vec = inputPile[i];
                if (vec == dir)
                {
                    inputPile.Remove(vec);
                }
            }

        }
        if (inputPile.Count > 0)
        {
            // refresh orientation to the last maintained key or key the last one if there are no more inputs
            RefreshOrientationSprite(inputPile[inputPile.Count - 1]);
        }
    }

    public void ClearInputPile()
    {
        inputPile.Clear();
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
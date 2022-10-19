using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

enum Orientation
{
    front, back, left, right
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap[] obstacleTilemaps;
    [SerializeField] private Animator animator;

    [Header("Movement variables")]
    [SerializeField] private Vector3Int tilemapPosition;
    [SerializeField] private int walkFrameCooldown;
    [SerializeField] private List<Vector2Int> inputPile;
    private int frameCounter;
    private Vector3Int targetTilemapPosition;
    private Vector2Int direction;

    [Header("States")]
    [SerializeField] private Orientation orientation;
    

    private void Awake()
    {
        Assert.IsNotNull(grid);
        if (!animator) animator = GetComponentInChildren<Animator>();

        targetTilemapPosition = Vector3Int.zero;
        direction = Vector2Int.zero;
        frameCounter = 0;
        orientation = Orientation.front;
        inputPile = new List<Vector2Int>();

        // set position exactly on a tile
        tilemapPosition = grid.WorldToCell(transform.position);
        transform.position = grid.CellToWorld(tilemapPosition);
    }

    private void Update()
    {
        if (frameCounter < walkFrameCooldown)
        {
            frameCounter++;
        }
    }

    private void FixedUpdate()
    {   
        if (frameCounter >= walkFrameCooldown)
        {
          Move();
        }
    }

    private void Move()
    {
        if (inputPile.Count > 0)
        {
            // set direction as the top of the input pile
            direction = inputPile[inputPile.Count - 1];
            // calculate target position
            targetTilemapPosition = grid.WorldToCell(transform.position) + (Vector3Int)direction;

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

            // move the player if no obstacle was found
            transform.position = grid.CellToWorld(targetTilemapPosition);
            tilemapPosition = grid.WorldToCell(transform.position);

            // reset counter
            frameCounter = 0;
        }
    }

    private void RefreshOrientation(Vector2Int direction)
    {
        if (direction == Vector2Int.down)
        {
            orientation = Orientation.front;
        } else if (direction == Vector2Int.up)
        {
            orientation = Orientation.back;
        } else if (direction == Vector2Int.right)
        {
            orientation = Orientation.right;
        } else if (direction == Vector2Int.left) 
        {
            orientation = Orientation.left;
        }

        animator.SetFloat("X", direction.x);
        animator.SetFloat("Y", direction.y);
    }


    public void AddMovementInPile(Vector2Int dir)
    {
        inputPile.Add(dir);
        RefreshOrientation(dir);
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
            RefreshOrientation(inputPile[inputPile.Count - 1]);
        }
    }

    public void ClearInputPile()
    {
        inputPile.Clear();
    }
}
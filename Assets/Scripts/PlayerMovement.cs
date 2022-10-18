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

    [Header("Movement variables")]
    [SerializeField] private Vector3Int tilemapPosition;
    [SerializeField] private int walkFrameCooldown;
    private int frameCounter;
    private Vector3Int targetTilemapPosition;
    private Vector2Int direction;

    [Header("States")]
    [SerializeField] private Orientation orientation;
    

    private void Awake()
    {
        Assert.IsNotNull(grid);
  

        targetTilemapPosition = Vector3Int.zero;
        direction = Vector2Int.zero;
        frameCounter = 0;
        orientation = Orientation.front;

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
        if (direction != Vector2Int.zero)
        {
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

    private void ChangeOrientation()
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
    }

    public void SetDirection(Vector2Int dir)
    {
        Debug.Log(dir);
        if (dir != Vector2Int.zero)
        {
            ChangeOrientation();
        }
        direction = dir;
    }
}

internal class List<T>
{
}
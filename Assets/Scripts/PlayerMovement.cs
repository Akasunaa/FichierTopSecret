using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

enum Orientation
{
    front, back, left, right
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Grid grid;

    [Header("Movement variables")]
    [SerializeField] private Vector3Int tilemapPosition;
    private Vector3Int targetTilemapPosition;

    [Header("States")]
    [SerializeField] private Orientation orientation;
    

    private void Awake()
    {
        Assert.IsNotNull(grid);

        // set position exactly on a tile
        targetTilemapPosition = Vector3Int.zero;
        tilemapPosition = grid.WorldToCell(transform.position);
        transform.position = grid.CellToWorld(tilemapPosition);

        orientation = Orientation.front;
    }

    private void FixedUpdate()
    {
        transform.position = grid.CellToWorld(targetTilemapPosition);
        tilemapPosition = grid.WorldToCell(transform.position);
    }

    public void MovementOrder(Vector2Int direction)
    {
        targetTilemapPosition = grid.WorldToCell(transform.position) + (Vector3Int) direction;
        /*if (direction.x == 0 ^ direction.y == 0)
        {
            // one component of direction is null, only one direction was input
            if (direction.x > 0)
            {
                orientation = Orientation.right;
            }
            if (direction.x < 0)
            {
                orientation = Orientation.left;
            }
            if (direction.y > 0)
            {
                orientation = Orientation.back;
            }
            if (direction.y < 0)
            {
                orientation = Orientation.front;
            }
        } else if (direction.x != 0 && direction.y != 0)
        {

        }*/
    }
}

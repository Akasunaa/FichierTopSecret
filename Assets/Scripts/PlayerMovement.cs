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
    private Vector2Int direction;

    [Header("States")]
    [SerializeField] private Orientation orientation;
    

    private void Awake()
    {
        Assert.IsNotNull(grid);

        // set position exactly on a tile
        targetTilemapPosition = Vector3Int.zero;
        direction = Vector2Int.zero;
        tilemapPosition = grid.WorldToCell(transform.position);
        transform.position = grid.CellToWorld(tilemapPosition);


        orientation = Orientation.front;
    }

    private void FixedUpdate()
    {
        targetTilemapPosition = grid.WorldToCell(transform.position) + (Vector3Int)direction;
        transform.position = grid.CellToWorld(targetTilemapPosition);
        tilemapPosition = grid.WorldToCell(transform.position);
    }

    public void SetDirection(Vector2Int dir)
    {
        direction = dir;
    }
}

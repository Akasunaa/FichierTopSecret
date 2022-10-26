using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 gridOffset;
    public Vector2 GridOffset => gridOffset;

    [SerializeField] private Vector2Int gridSize;
    public Vector2Int GridSize => gridSize;
}

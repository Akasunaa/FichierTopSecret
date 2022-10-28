using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 gridOffset;
    public Vector2 GridOffset => gridOffset;

    [SerializeField] private Vector2 bottomLeft;
    public Vector2 BottomLeft => bottomLeft;

    [SerializeField] private Vector2 topRight;
    public Vector2 TopRight => topRight;
}

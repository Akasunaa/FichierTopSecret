using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/**
 * Used on the ground tilemap in the cosmic bin to make the shockwave follow the player
 */
public class ShockwaveManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [FormerlySerializedAs("renderer")] [SerializeField] private TilemapRenderer localRenderer;
    private static readonly int FocalPoint = Shader.PropertyToID("_FocalPoint");

    private void Awake()
    {
        if (!localRenderer) localRenderer = GetComponent<TilemapRenderer>();
    }

    private void Update()
    {
        var playerPos = player.position;
        localRenderer.material.SetVector(FocalPoint, new Vector4(playerPos.x, playerPos.y, 0, 0));
    }
}

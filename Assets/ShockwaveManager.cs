using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        if (!localRenderer) localRenderer = GetComponent<TilemapRenderer>();
    }

    private void Update()
    {
        localRenderer.material.SetVector("_FocalPoint", new Vector4(player.position.x, player.position.y, 0, 0));
    }
}

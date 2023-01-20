using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * Used on the ground tilemap in the cosmic bin to make the shockwave follow the player
 */
public class ShockwaveManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private new TilemapRenderer renderer;

    private void Awake()
    {
        if (!renderer) renderer = GetComponent<TilemapRenderer>();
    }

    private void Update()
    {
        renderer.material.SetVector("_FocalPoint", new Vector4(player.position.x, player.position.y, 0, 0));
    }
}

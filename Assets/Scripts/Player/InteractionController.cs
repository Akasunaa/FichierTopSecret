using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

public class InteractionController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Tilemap[] interactableTilemaps;

    private void Awake()
    {
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
        Assert.IsNotNull(interactableTilemaps);
    }

    private void Update()
    {
        InteractionOrder();
    }
    public void InteractionOrder()
    {
        Vector2Int orientation = playerMovement.GetOrientation();
        Vector2Int targetTilemapPosition = playerMovement.GetTilemapPosition() + orientation;


        //todo : to rework 

        // check if the cell is occupied by an interactible object
        if (interactableTilemaps != null)
        {
            foreach (Tilemap tilemap in interactableTilemaps)
            {
                if (tilemap.HasTile((Vector3Int) targetTilemapPosition))
                {
                    Debug.Log("An interactable item was found");
                }
            }
        }
    }
}

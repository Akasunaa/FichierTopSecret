using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    const float EPSILON = 0.1f;
    /**
     * Method checking a tile for a collider and returning it. If there are no collider on the tile, returns null.
     */
    public static GameObject CheckPresenceOnTile(Grid grid, Vector2 position, Vector2? size = null)
    {
        Collider2D hit;
        
        if (size == null)
        {
            hit = Physics2D.OverlapBox(position, (Vector2) grid.cellSize - Vector2.one * EPSILON, 0);
        } else
        {
            Vector2 colliderSize = (Vector2) size;
            hit = Physics2D.OverlapBox(position, colliderSize - Vector2.one * EPSILON, 0);
        }
        return hit?.gameObject;
    }


    /**
    * Method checking a tile for a collider and returning it with tiilemap position. If there are no collider on the tile, returns null.
    */
    public static GameObject? CheckPresenceOnTile(Grid grid, Vector3Int position, Vector2? size = null) //look if player can move to the target 
    {
        // using GetCellCenterWorld is very important to avoid locking on to the corner of the tile
        Collider2D hit;

        if (size == null)
        {
            hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), (Vector2) grid.cellSize - Vector2.one * EPSILON, 0);
        }
        else
        {
            Vector2 colliderSize = (Vector2)size;
            hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), colliderSize - Vector2.one * EPSILON, 0);
        }
        return hit?.gameObject;
    }




    public static void UpdateOrderInLayer(GameObject go)
    {
        // update order in layer
        LayerOrderManager layerManager;
        if ((layerManager = go.GetComponentInChildren<LayerOrderManager>()) != null)
        {
            // refresh the order in layer if the object has a layer manager
            layerManager.CalculateOrderInLayer();
        }
    }
}

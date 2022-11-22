using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class Utils
{
    /**
     * Method checking a tile for a collider and returning it. If there are no collider on the tile, returns null.
     */
    public static GameObject? CheckPresenceOnTile(Grid grid, Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapBox(position, grid.cellSize - Vector3.one * 0.1f, 0);
        return hit?.gameObject;
    }


    /**
    * Method checking a tile for a collider and returning it with tiilemap position. If there are no collider on the tile, returns null.
    */
    public static GameObject? CheckPresenceOnTileWithTilemap(Grid grid, Vector3Int position) //look if player can move to the target 
    {
        // using GetCellCenterWorld is very important to avoid locking on to the corner of the tile
        Collider2D hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), grid.cellSize - Vector3.one * 0.1f, 0);
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

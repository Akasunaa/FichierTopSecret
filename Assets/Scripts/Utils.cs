using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
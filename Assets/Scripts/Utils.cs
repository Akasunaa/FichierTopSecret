#nullable enable
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils
{
    public const string RootFolderName = "Test";
    private const float Epsilon = 0.1f;
    /**
     * Method checking a tile for a collider and returning it. If there are no collider on the tile, returns null.
     */
    public static GameObject? CheckPresenceOnTile(Grid grid, Vector2 position, Vector2? size = null)
    {
        Collider2D hit;
        
        if (size == null)
        {
            hit = Physics2D.OverlapBox(position, (Vector2) grid.cellSize - Vector2.one * Epsilon, 0);
        } else
        {
            var colliderSize = (Vector2) size;
            hit = Physics2D.OverlapBox(position, colliderSize - Vector2.one * Epsilon, 0);
        }
        return hit != null ? hit.gameObject : null;
    }

    /**
     * Method checking a tile for colliders and returning them. If there are no collider on the tile, returns null.
     */
    public static List<GameObject> CheckPresencesOnTile(Grid grid, Vector2 position, Vector2? size = null)
    {
        Collider2D[] hits = {};
        var _ = Physics2D.OverlapBoxNonAlloc(position, (size ?? grid.cellSize)- Vector2.one * Epsilon, 0, hits);

        return hits.Select(hit => hit.gameObject).ToList();
    }

    /**
    * Method checking a tile for a collider and returning it with tilemap position. If there are no collider on the tile, returns null.
    */
    public static GameObject? CheckPresenceOnTile(Grid grid, Vector3Int position, Vector2? size = null) //look if player can move to the target 
    {
        // using GetCellCenterWorld is very important to avoid locking on to the corner of the tile
        Collider2D hit;

        if (size == null)
        {
            hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), (Vector2) grid.cellSize - Vector2.one * Epsilon, 0);
        }
        else
        {
            var colliderSize = (Vector2)size;
            hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), colliderSize - Vector2.one * Epsilon, 0);
        }
        return hit != null ? hit.gameObject : null;
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

    /**
    Renvoie la position libre la plus proche de la position d'entrée s'il y en a une, renvoie null sinon
    position : position de départ
    size : taille de la position libre voulue
    depth : variable de récursion
    limit : nombre d'iteration, si on veut une case adjacente mettre "limite: 1" 
    */
    public static Vector3Int? NearestTileEmpty(Vector2Int position, Vector2? size = null, int depth = 1, int limit = 100)
    {
        if (depth > limit) { return null; }
        for (var i = -depth; i <= depth; i++)
        {
            for (var j = -depth; j <= depth; j++)
            {
                if (i != depth && i != -depth && j != -depth && j != depth) continue;
                
                if (CheckPresenceOnTile(SceneData.Instance.grid, new Vector3Int(position.x + i, position.y + j, 0), size) == null)
                {
                    return new Vector3Int(position.x + i, position.y + j, 0);
                }
            }
        }
        return NearestTileEmpty(position, size, depth + 1,limit);
    }

}

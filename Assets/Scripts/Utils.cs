using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting.FullSerializer;

public static class Utils
{
    public const string RootFolderName = "Game";
    public const string PlayerFolderName = "player";
    public const string CosmicbinFolderName = "Cosmicbin";
    const float EPSILON = 0.1f; // Used for collision detection on a tile to not detect neighbours tiles colliders
    public const uint MAX_READ_FILE_SCENELOAD = 40;
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
     * Method checking a tile for colliders and returning them. If there are no collider on the tile, returns null.
     */
    public static List<GameObject> CheckPresencesOnTile(Grid grid, Vector2 position, Vector2? size = null)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            position,
            (Vector2) (size == null ? grid.cellSize : size)- Vector2.one * EPSILON,
            0);

        return hits.Select(hit => hit.gameObject).ToList();
    }

    /**
    * Method checking a tile for a collider and returning it with tiilemap position. If there are no collider on the tile, returns null.
    */
    public static GameObject CheckPresenceOnTile(Grid grid, Vector3Int position, Vector2? size = null) //look if player can move to the target 
    {
        // using GetCellCenterWorld is very important to avoid locking on to the corner of the tile
        Collider2D hit;

        if (size == null)
        {
            hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), (Vector2) grid.cellSize - Vector2.one * EPSILON, 0, LayerMask.GetMask("Default"));
        }
        else
        {
            Vector2 colliderSize = (Vector2)size;
            hit = Physics2D.OverlapBox(grid.GetCellCenterWorld(position), colliderSize - Vector2.one * EPSILON, 0, LayerMask.GetMask("Default"));
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

    /**
    Renvoie la position libre la plus proche de la position d'entree s'il y en a une, renvoie null sinon
    position : position de d�part
    size : taille de la position libre voulue
    depth : variable de r�cursion
    limit : nombre d'iteration, si on veut une case adjacente mettre "limite: 1" 
    */
    public static Vector2Int? NearestTileEmpty(Vector2Int position, Vector2? size = null, int depth = 1, int limit = 1000)
    {
        if (depth > limit) { return null; }
        for (int i = -depth; i <= depth; i++)
        {
            for (int j = -depth; j <= depth; j++)
            {
                if(i==depth || i==-depth || j==-depth || j == depth)
                {
                    if (Utils.CheckPresenceOnTile(SceneData.Instance.grid, new Vector3Int(position.x + i, position.y + j, 0), size) == null)
                    {
                        return new Vector2Int(position.x + i, position.y + j);
                    }
                }                 
            }
        }
        return NearestTileEmpty(position, size, depth + 1,limit);
    }

    // Utils funtions on path
    public static string RelativePath(string absolutePath)
    {
        return absolutePath[Application.streamingAssetsPath.Length..].Replace('\\', '/');
    }
    
    public static string RelativePath(FileInfo fi)
    {
        
        return fi.FullName[Application.streamingAssetsPath.Length..].Replace('\\', '/');
    }

    public static string SceneName(string relativePath)
    {
        string moreRelative = relativePath.Substring(("/" + RootFolderName + "/").Length);
        return moreRelative.Split('/').First();
    }
    
    public static string SceneName(FileInfo fi)
    {
        string relativePath = RelativePath(fi.FullName);
        string moreRelative = relativePath.Substring(("/" + RootFolderName + "/").Length);
        return moreRelative.Split('/').First();
    }

    public static string SceneRelativePath(string relativePath)
    {
        string moreRelative = relativePath.Substring(("/" + RootFolderName + "/").Length);
        return String.Join('/', moreRelative.Split('/').Skip(1));
    }
    
    public static string SceneRelativePath(FileInfo fi)
    {
        string relativePath = RelativePath(fi.FullName);
        string moreRelative = relativePath.Substring(("/" + RootFolderName + "/").Length);
        return String.Join('/', moreRelative.Split('/').Skip(1));
    }

    public static string FileName(string relativePath)
    {
        return relativePath.Split('/')[^1];
    }

    public static bool DeleteFile(FileInfo fi)
    {
        try
        {
            fi.Delete();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool DeleteFile(string relativePath)
    {
        return DeleteFile(Application.streamingAssetsPath + relativePath);
    }

    /// <summary>
    /// Function that handles the comparison between the integer file value and the integer property value of an NPC.
    /// Its main point is to avoid overflow-type errors.
    /// </summary>
    /// <param name="fileValue">Value obtained in the file that is being tested.</param>
    /// <param name="inspectorValue">Value setup in the NPC's inspector.</param>
    /// <param name="isSuperiorTest">Wether or not the test is fileValue is inferior to inspectorValue.</param>
    /// <returns>True if the test is correct, False otherwise.</returns>
    public static bool NPCCompare(string fileValue, string inspectorValue, bool isSuperiorTest)
    {
        List<string> numbers = new List<string>() { "0", "1", "2","3","4","5","6","7","8","9","10" };
        int integerValue;
        int.TryParse(fileValue, out integerValue);
        if(integerValue == 0 && fileValue.Any(c => c != 0)) //if the parsed value is 0 BUT the string value contains something different than 0, then we have overflow 
        {
            if(!fileValue.Any(c => numbers.Contains(c.ToString()))) //we test if the value is only made of numbers
            {
                return false;
            }
            return true;
        }
        int conditionValue;
        int.TryParse(inspectorValue, out conditionValue);
        if (isSuperiorTest)
        {
            if (integerValue < conditionValue)
            {
                return true;
            }
        }
        else
        {
            if(integerValue > conditionValue) 
            { 
                return true;
            }
        }
        return false;
    }
}

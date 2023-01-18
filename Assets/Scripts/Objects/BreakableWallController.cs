using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script used by walls destroyable by the explosives
/// </summary>
public class BreakableWallController : MonoBehaviour
{
    [SerializeField] private string levelName;

    private void Start()
    {
        Debug.Log("EXPLOSIVES : READ VALUE IN BREAKABLE WALL : " + PlayerPrefs.GetString("HasDetonated"));
        if (PlayerPrefs.GetString("HasDetonated")!=null && PlayerPrefs.GetString("HasDetonated").Contains(levelName))
        {
            DestroyWall();
        }
    }

    /// <summary>
    /// Function that handle the destruction of the wall
    /// </summary>
    private void DestroyWall()
    {
        Debug.Log("BREAKABLE WALLS : DESTROYING WALL");
        gameObject.SetActive(false);
    }
}

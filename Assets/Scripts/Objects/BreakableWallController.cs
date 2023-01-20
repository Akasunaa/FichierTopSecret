using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script used by walls destroyable by the explosives
/// </summary>
public class BreakableWallController : MonoBehaviour
{
    [SerializeField] private string levelName;
    [Header("Element hidden by breakable wall")]
    [SerializeField] private GameObject hiddenGameElement;

    private void Start()
    {
        string hasDetonated = PlayerPrefs.GetString("HasDetonated");
        if (string.IsNullOrEmpty(hasDetonated) || Utils.SceneName(Utils.RelativePath(hasDetonated)) != levelName)
        {
            hiddenGameElement.SetActive(false);
        }
    }

    /// <summary>
    /// Function that handle the destruction of the wall, called by LevelManager when the explosive has detonated in the appropriate room
    /// </summary>
    public void DestroyWall()
    {
        Debug.Log("BREAKABLE WALLS : DESTROYING WALL");
        gameObject.SetActive(false);
        hiddenGameElement.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportObjectController : ModifiableController, Interactable
{

    [SerializeField] TeleportObjectController pairTeleport;
    public static event Action deletedTeleport;

    //TeleportObjectController.star

    private void Awake()
    {
        deletedTeleport +=  searchPair;
    }

    public bool canBeInteracted { get; set; }

    public void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2Int? target = Utils.NearestTileEmpty((Vector2Int)SceneData.Instance.grid.WorldToCell(pairTeleport.gameObject.transform.position), limit : 1);
        if (target != null)
            player.transform.position = new Vector3(target.Value.x, target.Value.y, 0); //add feedback ?
    }

    private void Start()
    {
        searchPair();
    }

    /**
    * Search a pair : another if possible, himself if alone
    */
    private void searchPair()
    {
        TeleportObjectController[] teleports = FindObjectsOfType<TeleportObjectController>();
        if (teleports.Length > 1)
        {
            foreach (TeleportObjectController teleport in teleports)
            {
                if (teleport != this) { pairTeleport = teleport; break; }
            }
        }
        else { pairTeleport = this; }
    }

    /**
     * if destroy, all teleport search a new pair 
     */
    private void OnDestroy()
    {
        deletedTeleport -= searchPair;
        deletedTeleport?.Invoke();
    }
}

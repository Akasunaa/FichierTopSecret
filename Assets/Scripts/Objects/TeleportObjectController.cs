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
        Vector3Int? target = Utils.NearestTileEmpty((Vector2Int)SceneData.Instance.grid.WorldToCell(pairTeleport.gameObject.transform.position), limit : 2);
        if (target != null)
            player.transform.position = (Vector3)target;
    }

    private void Start()
    {
        searchPair();
    }

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

    private void OnDestroy()
    {
        deletedTeleport -= searchPair;
        deletedTeleport?.Invoke();
    }
}

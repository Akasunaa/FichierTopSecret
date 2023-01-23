using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportObjectController : ModifiableController, Interactable
{
    [SerializeField] TeleportObjectController pairTeleport;
    public static event Action deletedTeleport;
    [SerializeField] Sprite spriteBlue;
    [SerializeField] Sprite spriteOrange;
    SpriteRenderer spriteRenderer;
    //TeleportObjectController.star

    private void Awake()
    {
        deletedTeleport += deleted;
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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = spriteBlue;
        pairTeleport = this;
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
                if (teleport != this && teleport.pairTeleport== teleport) { 
                    pairTeleport = teleport;
                    teleport.pairTeleport = this;
                    spriteRenderer.sprite = spriteOrange;
                    break; 
                }
            }
        }
    }

    private void deleted()
    {
        if(!pairTeleport.gameObject.activeSelf)
        {
            spriteRenderer.sprite = spriteBlue;
            pairTeleport = this;
        }
    }

    /**
     * if send to cosmicbin, all teleport search a new pair 
     */
    private void OnDisable()
    {
        pairTeleport = this;
        deletedTeleport?.Invoke();
        deletedTeleport -= deleted;
    }
}

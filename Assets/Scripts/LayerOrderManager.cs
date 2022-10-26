using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrderManager : MonoBehaviour
{
    /* Script to apply to any object in the that possesses a sprite renderer
       Sets a good order in layer and dynamically updates it if needed */

    [SerializeField] private bool isMoving;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Transform root;
    private int offset;

    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        // somehow the player always has an offset of 2 compared to the objects
        root = transform.root;
        offset = root.tag == "Player" ? -2 : 0;
    }

    private void Start()
    {
        spriteRenderer.sortingOrder = 0 + (-1 * SceneData.Instance.grid.WorldToCell(root.position).y) + offset;
        
        // if the object is static no need to keep the component updated
        if (!isMoving)
        {
            this.enabled = false;
        }
    }
        
    private void Update()
    {
        spriteRenderer.sortingOrder = 0 + (-1 * SceneData.Instance.grid.WorldToCell(root.position).y) + offset;
    }
}

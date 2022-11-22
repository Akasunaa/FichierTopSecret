using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script to apply to any object in the that possesses a sprite renderer
Sets a good order in layer and dynamically updates it if needed */

public class LayerOrderManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int offset;
    private Transform root;

    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        root = transform.root;
    }

    private void Start()
    {
        CalculateOrderInLayer();
    }

    public void CalculateOrderInLayer()
    {
        spriteRenderer.sortingOrder = 0 + (-1 * SceneData.Instance.grid.WorldToCell(transform.parent.position).y) + offset;
    }
}

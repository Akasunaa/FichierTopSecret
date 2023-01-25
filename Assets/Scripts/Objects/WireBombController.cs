using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBombController : ModifiableController
{
    [SerializeField] int order;
    [HideInInspector] public static event Action<int> OnWireDestroy;

    private void OnDestroy()
    {
        OnWireDestroy?.Invoke(order);
    }

    void OnDisable()
    {
        OnWireDestroy?.Invoke(order);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BombController : ModifiableController
{
    [SerializeField] int numberOfWire;
    int lastWireDestroyed=0;

    private void Awake()
    {
        WireBombController.OnWireDestroy += WireDestroyed;
    }

    private void WireDestroyed(int order)
    {
        if(order != lastWireDestroyed+1) { print("BOMB EXPLODE");}
        else {
            lastWireDestroyed = order;
            if(lastWireDestroyed == numberOfWire) { print("DESAMORCED"); }
        }
    }



    private void OnDestroy()
    {
        WireBombController.OnWireDestroy -= WireDestroyed;
    }


    
}

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
        if(order != lastWireDestroyed+1) 
        { 
            print("BOMB EXPLODE");
            //DEATH TRIGGER
            //We trigger death here
            //we recuperate the ui :
            GameObject ui = GameObject.FindGameObjectWithTag("UI");
            //we get the correcte component :
            //TODO
            //we launch the right function :
            //TODO
        }
        else {
            lastWireDestroyed = order;
            if(lastWireDestroyed == numberOfWire) 
            { 
                print("DESAMORCED");
                //TRIGGER VICTORY
            }
        }
    }

    private void OnDestroy()
    {
        WireBombController.OnWireDestroy -= WireDestroyed;
    }    
}

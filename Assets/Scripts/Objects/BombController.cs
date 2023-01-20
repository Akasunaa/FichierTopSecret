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
        var ui = GameObject.FindGameObjectWithTag("UI");
        var playerDeathScreenController = ui.GetComponent<GameOverScreenController>();
        if(order != lastWireDestroyed+1) 
        { 
            print("BOMB EXPLODE");
            //DEATH TRIGGER
            //We trigger death here
            //we recuperate the ui :
            // var ui = GameObject.FindGameObjectWithTag("UI");
            //we get the correcte component :
            //we launch the right function :
            playerDeathScreenController.OnGameOver(GameOverScreenController.GameOverType.BombDetonated);
        }
        else {
            lastWireDestroyed = order;
            if(lastWireDestroyed == numberOfWire) 
            { 
                print("DESAMORCED");
                //TRIGGER VICTORY
                playerDeathScreenController.OnGameOver(GameOverScreenController.GameOverType.Victory);
            }
        }
    }

    private void OnDestroy()
    {
        WireBombController.OnWireDestroy -= WireDestroyed;
    }    
}

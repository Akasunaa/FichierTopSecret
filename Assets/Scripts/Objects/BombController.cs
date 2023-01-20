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
            StartCoroutine(BombExplode());
        }
        else {
            lastWireDestroyed = order;
            if(lastWireDestroyed == numberOfWire) 
            { 
                print("DESAMORCED");
                //TRIGGER VICTORY
                var ui = GameObject.FindGameObjectWithTag("UI");
                var playerDeathScreenController = ui.GetComponent<GameOverScreenController>();
                playerDeathScreenController.OnGameOver(GameOverScreenController.GameOverType.Victory);

            }
        }
    }

    /// <summary>
    /// Explosion of the bomb, through the camera shake and white fade.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BombExplode()
    {
        CameraShaker.Instance.CameraShake(4f, 0.25f);
        yield return new WaitForSeconds(4f);
        var ui = GameObject.FindGameObjectWithTag("UI");
        var playerDeathScreenController = ui.GetComponent<GameOverScreenController>();
        playerDeathScreenController.OnGameOver(GameOverScreenController.GameOverType.BombDetonated);
    }

    private void OnDestroy()
    {
        WireBombController.OnWireDestroy -= WireDestroyed;
    }    
}

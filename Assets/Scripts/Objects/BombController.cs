using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BombController : ModifiableController
{
    [SerializeField] int numberOfWire;
    int lastWireDestroyed=0;

    private bool isDetonating;

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
            isDetonating = true;
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

    private void OnApplicationFocus(bool focus)
    {
        if(isDetonating)
        {
            isDetonating= false;
            StartCoroutine(BombExplode());
        }
    }

    /// <summary>
    /// Explosion of the bomb, through the camera shake and white fade.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BombExplode()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().StopMovement();
        var ui = GameObject.FindGameObjectWithTag("UI");
        CameraShaker.Instance.CameraShake(4f, 0.25f);
        ui.GetComponent<WhiteFadeController>().StartWhiteFade(4f);
        yield return new WaitForSeconds(4f);
        var playerDeathScreenController = ui.GetComponent<GameOverScreenController>();
        playerDeathScreenController.OnGameOver(GameOverScreenController.GameOverType.BombDetonated);
    }

    private void OnDestroy()
    {
        WireBombController.OnWireDestroy -= WireDestroyed;
    }    
}

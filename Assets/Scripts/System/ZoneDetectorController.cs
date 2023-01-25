using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Class used to handle zones of detection that, if the player passes through, acctivates a certain cinematic
/// </summary>
public class ZoneDetectorController : MonoBehaviour
{
    [Header("Cinematic element to activate upon detection")]
    [SerializeField] private GameObject activableElement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") //when the player is detected, we will deactivate the relevant gameobjects
        {
            PlayerPresenceTriggeredFunction();
        }
    }

    private void PlayerPresenceTriggeredFunction()
    {
        Debug.Log("PLAYER DETECTED");
        activableElement.SetActive(true);
    }
}

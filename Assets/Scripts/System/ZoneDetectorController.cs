using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to handle zones of detection that, if the player passes through, save a data in PlayerPrefs
/// </summary>
public class ZoneDetectorController : MonoBehaviour
{
    [Header("Deactivable Elements upon detection")]
    [SerializeField] private GameObject[] deactivableElements;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") //when the player is detected, we will deactivate the relevant gameobjects
        {
            Debug.Log("PLAYER DETECTED");
            foreach(var element in deactivableElements)
            {
                element.SetActive(false);
            }
        }
    }
}

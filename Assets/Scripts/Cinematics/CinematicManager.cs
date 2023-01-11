using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  This script will handle the opening cinematic
 *      One of its main power is to keep the player from doing anything while the cinematic is not over
 */
public class CinematicManager : MonoBehaviour
{
    [Header("Cinematic Informations")]
    [SerializeField] private float cinematicLength;
    [SerializeField] private CinematicData cinematicData;
    private InputController playerController;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>();
        StartCoroutine(StartCinematic());
    }

    /**
     *  Function that will start the cinematic saved in cinematicData 
     */
    private IEnumerator StartCinematic()
    {
        playerController.StopMovement(); //we prevent the player from doing anything before the cinematic is finished
        yield return new WaitForSeconds(cinematicLength);
        playerController.RestartMovement();
    }
}

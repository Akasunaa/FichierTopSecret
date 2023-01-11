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
    [SerializeField] private CinematicData cinematicData;
    private float cinematicLength;

    private InputController playerController;
    private GameObject cam;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        if(cinematicData != null)
        {
            StartCoroutine(StartCinematic());
        }
    }

    /**
     *  Function that will start the cinematic saved in cinematicData 
     */
    private IEnumerator StartCinematic()
    {
        cinematicLength = cinematicData.GetCinematicLength();
        playerController.StopMovement(); //we prevent the player from doing anything before the cinematic is finished
        cam.GetComponent<Animator>().Play(cinematicData.GetCinematicAnimationStateName());
        yield return new WaitForSeconds(cinematicLength);
        playerController.RestartMovement();
    }
}

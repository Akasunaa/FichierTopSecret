using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/**
 *  This script will handle the opening cinematic
 *      One of its main power is to keep the player from doing anything while the cinematic is not over
 */
public class CinematicManager : MonoBehaviour
{
    [Header("Cinematic Elements")]
    [SerializeField] private PlayableAsset cinematicData;
    [SerializeField] private string cinematicPlayerPrefs; //this playerPref ensures that the cinematic has not already been played
    private PlayableDirector cinematicDirector;
    private GameObject player;

    private void Awake()
    {
        cinematicDirector = GetComponent<PlayableDirector>();
        player = GameObject.FindGameObjectWithTag("Player");
        if(PlayerPrefs.GetString(cinematicPlayerPrefs) == "TRUE")
        {
            GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
        }
        else if(cinematicData!=null && cinematicDirector != null)
        {
            StartCoroutine(StartCinematic());
        }
    }


    /**
     *  Function that will start the cinematic saved in cinematicData 
     */
    private IEnumerator StartCinematic()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        cinematicDirector.playableAsset = cinematicData;
        cinematicDirector.Play();
        yield return new WaitForSeconds((float)cinematicData.duration);
        player.GetComponent<PlayerInput>().enabled = true;
        PlayerPrefs.SetString(cinematicPlayerPrefs, "TRUE");
        PlayerPrefs.Save();
    }
}

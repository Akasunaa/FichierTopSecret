using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

/**
 *  This script will handle the opening cinematic
 *      One of its main power is to keep the player from doing anything while the cinematic is not over
 */
public class CinematicManager : MonoBehaviour
{
    [Header("Cinematic Elements")]
    [SerializeField] private PlayableAsset cinematicData;
    [SerializeField] private string cinematicPlayerPrefs; //this playerPref ensures that the cinematic has not already been played
    [SerializeField] private bool playCinematic;
    private PlayableDirector _cinematicDirector;
    private GameObject _player;
    private GameObject _ui;

    private void Awake()
    {
        _cinematicDirector = GetComponent<PlayableDirector>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _ui = GameObject.FindGameObjectWithTag("UI");
        if(PlayerPrefs.GetString(cinematicPlayerPrefs) == "TRUE" || (Application.isEditor && !playCinematic))
        {
            _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
        }
        else if(cinematicData!=null && _cinematicDirector != null)
        {
            StartCoroutine(StartCinematic());
        }
    }


    /**
     *  Function that will start the cinematic saved in cinematicData 
     */
    private IEnumerator StartCinematic()
    {
        if (Application.isEditor && !playCinematic) yield break;
        
        var rulerCanvas = _ui.GetComponent<Ruler>().rulerCanvas;
        rulerCanvas.SetActive(false);
        _player.GetComponent<PlayerInput>().enabled = false;
        _cinematicDirector.playableAsset = cinematicData;
        _cinematicDirector.Play();
        yield return new WaitForSeconds((float)cinematicData.duration);
        _player.GetComponent<PlayerInput>().enabled = true;
        PlayerPrefs.SetString(cinematicPlayerPrefs, "TRUE");
        PlayerPrefs.Save();
        rulerCanvas.SetActive(true);
    }
}

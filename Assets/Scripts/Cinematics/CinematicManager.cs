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
    //[SerializeField] private bool playCinematic;
    private PlayableDirector _cinematicDirector;
    private GameObject _player;
    private GameObject _ui;
    private bool cinematicIsPlaying;

    private void Awake()
    {
        cinematicIsPlaying = false;
        _cinematicDirector = GetComponent<PlayableDirector>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _ui = GameObject.FindGameObjectWithTag("UI");
        if(PlayerPrefs.GetString(cinematicPlayerPrefs) == "TRUE" && _ui.GetComponent<DialogueUIController>().cinematicCanvas /*|| (Application.isEditor && !playCinematic)*/)
        {
            _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
        }
        else if(cinematicData!=null && _cinematicDirector != null)
        {
            StartCoroutine(StartCinematic());
        }
    }

    private void Update()
    {
        if (cinematicIsPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            StopCinematic();
        }
    }

    private void StopCinematic()
    {
        StopAllCoroutines();
        _cinematicDirector.time = _cinematicDirector.playableAsset.duration;
        _cinematicDirector.Evaluate();
        _cinematicDirector.Stop();
        _player.GetComponent<PlayerInput>().enabled = true;
        PlayerPrefs.SetString(cinematicPlayerPrefs, "TRUE");
        PlayerPrefs.Save();
        var rulerCanvas = _ui.GetComponent<Ruler>().rulerCanvas;
        rulerCanvas.SetActive(true);
    }

    /**
     *  Function that will start the cinematic saved in cinematicData 
     */
    private IEnumerator StartCinematic()
    {
        if (Application.isEditor /*&& !playCinematic*/)
        {
            _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
            yield break;
        }
        _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(true);
        cinematicIsPlaying = true;
        var rulerCanvas = _ui.GetComponent<Ruler>().rulerCanvas;
        rulerCanvas.SetActive(false);
        _player.GetComponent<PlayerInput>().enabled = false;
        _cinematicDirector.playableAsset = cinematicData;
        _cinematicDirector.Play();
        yield return new WaitForSeconds((float)cinematicData.duration);
        _player.GetComponent<PlayerInput>().enabled = true;
        PlayerPrefs.SetString(cinematicPlayerPrefs, "TRUE");
        PlayerPrefs.Save();
        cinematicIsPlaying=false;
        _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
        rulerCanvas.SetActive(true);
    }
}

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

/// <summary>
/// This script will handle cinematics played when a scene is instantiated
/// One of its main power is to keep the player from doing anything while the cinematic is not over
/// </summary>
public class CinematicManager : MonoBehaviour
{
    [Header("Cinematic Elements")]
    [SerializeField] protected PlayableAsset cinematicData;
    [SerializeField] protected string cinematicPlayerPrefs; //this playerPref ensures that the cinematic has not already been played
    //[SerializeField] private bool playCinematic;
    protected PlayableDirector _cinematicDirector;
    protected GameObject _player;
    protected GameObject _ui;
    protected GameObject _uiConstant;
    protected bool cinematicIsPlaying;

    protected void Awake()
    {
        cinematicIsPlaying = false;
        _cinematicDirector = GetComponent<PlayableDirector>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _ui = GameObject.FindGameObjectWithTag("UI");
        _uiConstant = GameObject.FindGameObjectWithTag("UI-CONSTANT");
        if(PlayerPrefs.GetString(cinematicPlayerPrefs) == "TRUE" && _ui.GetComponent<DialogueUIController>().cinematicCanvas /*|| (Application.isEditor && !playCinematic)*/)
        {
            _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
        }
        else if(cinematicData!=null && _cinematicDirector != null)
        {
            StartCoroutine(StartCinematic());
        }
    }

    protected void Update()
    {
        if (cinematicIsPlaying && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            StopCinematic();
        }
    }

    /// <summary>
    /// Function that stops the cinematic in its track, triggered by a push of a button (SPACE, ESC, or RETURN)
    /// </summary>
    protected void StopCinematic()
    {
        StopAllCoroutines();
        //We stop the cinematic and go to its end :
        _cinematicDirector.time = _cinematicDirector.playableAsset.duration;
        _cinematicDirector.Evaluate();
        _cinematicDirector.Stop();
        //----------------------------------------
        //we now enable/disable all the relevant data :
        _player.GetComponent<PlayerInput>().enabled = true;
        PlayerPrefs.SetString(cinematicPlayerPrefs, "TRUE");
        PlayerPrefs.Save();
        var rulerCanvas = _uiConstant.GetComponent<Ruler>().rulerCanvas;
        rulerCanvas.SetActive(true);
    }

    /// <summary>
    /// Function that will start the cinematic saved in cinematicData 
    /// </summary>    
    protected virtual IEnumerator StartCinematic()
    {
        if (Application.isEditor /*&& !playCinematic*/)
        {
            _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
            yield break;
        }
        _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(true);
        cinematicIsPlaying = true;
        var rulerCanvas = _uiConstant.GetComponent<Ruler>().rulerCanvas;
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

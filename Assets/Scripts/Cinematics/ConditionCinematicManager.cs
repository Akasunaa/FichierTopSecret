using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

/// <summary>
/// This script will handle cinematics that require conditions (PlayerPrefs) to be played
/// If a certain playerPrefs has been modified, the cinematic changes
/// </summary>
public class ConditionCinematicManager : CinematicManager
{
    [Header("Condition cinematic elements")]
    [SerializeField] private PlayableAsset conditionCinematic;
    [SerializeField] private string conditionPlayerPrefsName;

    /// <summary>
    /// Function that will start the cinematic saved in cinematicData 
    /// </summary>    
    protected override IEnumerator StartCinematic()
    {
        if (Application.isEditor /*&& !playCinematic*/)
        {
            _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
            yield break;
        }
        //we select the correct animation :
        if(PlayerPrefs.HasKey(conditionPlayerPrefsName) && PlayerPrefs.GetString(conditionPlayerPrefsName) == "TRUE")
        {
            _cinematicDirector.playableAsset = conditionCinematic;
        }
        else
        {
            _cinematicDirector.playableAsset = cinematicData;
        }

        _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(true);
        cinematicIsPlaying = true;
        var rulerCanvas = _ui.GetComponent<Ruler>().rulerCanvas;
        rulerCanvas.SetActive(false);
        _player.GetComponent<PlayerInput>().enabled = false;
        _cinematicDirector.Play();
        yield return new WaitForSeconds((float)cinematicData.duration);
        _player.GetComponent<PlayerInput>().enabled = true;
        PlayerPrefs.SetString(cinematicPlayerPrefs, "TRUE");
        PlayerPrefs.Save();
        cinematicIsPlaying = false;
        _ui.GetComponent<DialogueUIController>().cinematicCanvas.SetActive(false);
        rulerCanvas.SetActive(true);
    }

}

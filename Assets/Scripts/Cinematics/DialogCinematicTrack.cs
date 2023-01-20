using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// This script is used to control the creation of tracks handling the dialogs in a cutscene
/// </summary>
[TrackBindingType(typeof(TextMeshProUGUI))]
[TrackClipType(typeof(DialogCinematicClip))]
public class DialogCinematicTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogCinematicTrackMixer>.Create(graph, inputCount);
    }
}

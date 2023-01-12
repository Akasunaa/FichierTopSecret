using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

/**
 *  This script is used to control the creation of tracks handling the dialogs in a cutscene
 */
[TrackBindingType(typeof(TextMeshProUGUI))]
[TrackClipType(typeof(DialogCinematicClip))]
public class DialogCinematicTrack : TrackAsset
{

}

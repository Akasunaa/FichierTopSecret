using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

/// <summary>
/// This script will control how the Dialog Cinematic Track behaves during a cinematic
/// </summary>
public class DialogCinematicBehaviour : PlayableBehaviour
{
    public string dialogText; //This is the data received from DialogCinematicClip

    /// <summary>
    /// Function called every frame of the cinematic that will handle the display of the cinematic dialog
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="info"></param>
    /// <param name="playerData"></param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        text.text = dialogText;
        text.color = new Color(1,1,1,info.weight); //this line controls the appearance of the dialog text, with info.weight being the informations defined in the clip's inspector that will control the transparency of the text
    }
}

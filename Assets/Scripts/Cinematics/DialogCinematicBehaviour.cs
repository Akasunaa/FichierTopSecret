using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

/**
 *  This script will control how the Dialog Cinematic Track behaves during a cinematic
 */
public class DialogCinematicBehaviour : PlayableBehaviour
{
    public string dialogText; //This is the data received from DialogCinematicClip

    /**
     *  Function called every frame of the cinematic that will handle the display of the cinematic dialog
     */
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        text.text = dialogText;
        text.color = new Color(1,1,1,info.weight); //this line controls the appearance of the dialog text, with info.weight being the informations defined in the clip's inspector that will control the transparency of the text
    }
}

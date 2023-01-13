using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Playables;

/**
 *  This script will handle the behaviour of the entire dialog track, mainly to allow for no texts being displayed when no clips are loaded at a certain frame
 * 
 */
public class DialogCinematicTrackMixer : PlayableBehaviour
{
    /**
     *  Function called every frame of the cinematic that will handle the display of the cinematic dialog
     */
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        string currentText = "";
        float currentAlpha = 0f;

        if (!text) { return; } //if no text is loaded through clips, we quit there with empty text and alpha transparent

        //if we're here, it means there is a clip loaded at the current frame
        int inputCount = playable.GetInputCount(); //we get the number of clips in the current track
        for (int clipIndex = 0; clipIndex < inputCount; clipIndex++)
        {
            float inputWeight = playable.GetInputWeight(clipIndex);
            if(inputWeight > 0) //if the input weight is >0 then it's the current text that should be displayed
            {
                ScriptPlayable<DialogCinematicBehaviour> inputPlayable = (ScriptPlayable<DialogCinematicBehaviour>)playable.GetInput(clipIndex);
                DialogCinematicBehaviour input = inputPlayable.GetBehaviour();
                currentText = input.dialogText;
                currentAlpha = inputWeight;
            }
        }

        text.text = currentText;
        text.color = new Color(1, 1, 1, currentAlpha); //this line controls the appearance of the dialog text, with info.weight being the informations defined in the clip's inspector that will control the transparency of the text
    }
}

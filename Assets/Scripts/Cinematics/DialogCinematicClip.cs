using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/**
 *  Script used to create clips of Cinematic Dialogs used in the game's cinematics
 *      
 */
public class DialogCinematicClip : PlayableAsset
{
    [TextArea(3,10)]
    public string dialogText; //dialog data that will be sent to the associated behaviour 

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        //we begin by creating the associated playable behaviour used to display said clip :
        var playable = ScriptPlayable<DialogCinematicBehaviour>.Create(graph);
        DialogCinematicBehaviour dialogCinematicBehaviour = playable.GetBehaviour();

        //we feed the dialogText to the behaviour
        dialogCinematicBehaviour.dialogText = dialogText;

        return playable;
    }
}

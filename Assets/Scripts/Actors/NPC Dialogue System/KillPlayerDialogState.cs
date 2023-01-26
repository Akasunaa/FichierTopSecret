using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State
/// </summary>
[CreateAssetMenu(fileName = "Kill Player Dialog State", menuName = "States/Kill Player Dialog State")]
public class KillPlayerDialogState : DialogState
{
    public KillPlayerDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Inherited and overrided function that will handle the killing of the Player
    /// </summary>
    /// <returns>0 if the NPC has reached the end of its dialog speeches (and kills player), otherwise 1</returns>
    public override int ChangeSpeech()
    {
        interactionIndex++;
        if (interactionIndex < speech.Length)
        {
            currentSpeech = speech[interactionIndex];
            GetSpeechVariables(SM);
            return 1;
        }
        else //when reaching the end of the various speeches, the NPC will die
        {
            currentSpeech = speech[^1];
            GetSpeechVariables(SM);
            TriggerPlayerDeath();
            return 0;
        }
    }

    /// <summary>
    /// Actual trigger of the Player's Death.
    /// </summary>
    private void TriggerPlayerDeath()
    {
        GameObject ui = GameObject.FindGameObjectWithTag("UI");
        if (ui == null) return;

        //we get the correcte component :
        GameOverScreenController gameOverScreenController = ui.GetComponent<GameOverScreenController>();
        if (gameOverScreenController == null) return;

        //we launch the right function :
        gameOverScreenController.OnGameOver(GameOverScreenController.GameOverType.StreetThugDeath);
    }

}

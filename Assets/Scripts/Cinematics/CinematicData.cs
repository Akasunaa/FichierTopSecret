using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Property Dialog State", menuName = "Cinematics/Cinematic Data")]
public class CinematicData : ScriptableObject
{
    [Header("Cinematic elements")]
    [SerializeField] private float cinematicLength;
    [SerializeField] private string cinematicAnimationStateName;
    public float GetCinematicLength() { return cinematicLength; }
    public string GetCinematicAnimationStateName() { return cinematicAnimationStateName; }
}

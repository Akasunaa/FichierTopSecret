using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Property Dialog State", menuName = "Cinematics/Cinematic Data")]
public class CinematicData : ScriptableObject
{
    [Header("")]
    [SerializeField] private float cinematicLength;
}

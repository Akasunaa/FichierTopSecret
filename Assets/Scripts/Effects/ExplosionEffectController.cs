using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script used by the explosion effects, instantiated and destroyed immediately after their animation played out
/// </summary>
public class ExplosionEffectController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(1.083f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/**
 *  Component handling the lamp's modifications and state according to its file
 *  Associated file : Lamp.txt
 *  Values in file :
 *      power : on/off
 */
public class LampObjectController :  ModifiableController
{
    [SerializeField] private Light2D light;

    private void Awake()
    {
        light.gameObject.SetActive(false);
    }

    public override void UpdateModification()
    {
        //For the lamp object, we test if its power is on or off
        if (properties.ContainsKey("power")) 
        {
            if (properties["power"] == "on")
            {
                light.gameObject.SetActive(true);
            }
            else if (properties["power"]=="off")
            {
                light.gameObject.SetActive(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.Assertions;
using Unity.VisualScripting;

/**
 *  Component handling the lamp's modifications and state according to its file
 *  Associated file : Lamp.txt
 *  Values in file :
 *      power : on/off
 */
public class LampObjectController :  ModifiableController, Interactable
{
    public bool canBeInteracted { get; set; }

    public void Interact()
    {
        if (TryGet("power", out bool power))
        {
            SetValue("power", !power);
            UpdateModification();
            UpdateFile();
        }
    }

    private Light2D[] lights;

    private void Awake()
    {
        lights = GetComponentsInChildren<Light2D>();
        Assert.AreNotEqual(0, lights.Length);
    }

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.Add("power", true);
        properties.Add("color", Color.white);
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For the lamp object, we test if its power is on or off
        if (TryGet("power", out bool power)) 
        {
            if (power)
            {
                foreach(Light2D light in lights)
                {
                    light.enabled = true;
                }
            }
            else
            {
                foreach(var light in lights)
                {
                    light.enabled = false;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.Assertions;

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
        if (properties.ContainsKey("power"))
        {
            if (properties["power"] == "off")
            {
                properties["power"] = "on";
            }
            else
            {
                properties["power"] = "off";
            }

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

    public override void setDefaultProperties()
    {
        properties.Add("power", "on");
        properties.Add("color", "white");
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For the lamp object, we test if its power is on or off
        if (properties.ContainsKey("power")) 
        {
            if (properties["power"] == "on")
            {
                foreach(Light2D light in lights)
                {
                    light.gameObject.SetActive(true);
                }
            }
            else if (properties["power"]=="off")
            {
                foreach(var light in lights)
                {
                    light.gameObject.SetActive(false);
                }
            }
        }
    }
}

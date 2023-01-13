using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
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
        if (!TryGet("power", out bool power)) return;
        
        SetValue("power", !power);
        UpdateModification();
        UpdateFile();
    }

    private Light2D[] _lights;

    private void Awake()
    {
        _lights = GetComponentsInChildren<Light2D>();
        Assert.AreNotEqual(0, _lights.Length);
    }

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.Add("power", new DicoValueProperty {IsImportant = true, Value = true});
        properties.Add("color", new DicoValueProperty {IsImportant = true, Value = Color.white});
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For the lamp object, we test if its power is on or off
        if (!TryGet("power", out bool power)) return;
        
        foreach(var light2D in _lights)
        {
            light2D.enabled = power;
        }
    }
}

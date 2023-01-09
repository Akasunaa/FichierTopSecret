using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Component handling the hologram's modifications and state according to its file
 *  Associated file : Hologram.txt
 *  Values in file :
 *      power : on/off
 */
public class HologramObjectController : ModifiableController
{
    [SerializeField] private GameObject hologram;

    private void Awake()
    {
        if (!hologram) hologram = transform.GetChild(0).gameObject;
    }

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.Add("power", new DicoValueProperty {IsImportant = true, Value = true});
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For the lamp object, we test if its power is on or off
        if (TryGet("power", out bool power))
        {
            hologram.SetActive(power);
        }
    }

    /**
     *  Function called by HologramConsoleController that will activate/deactivate the holo's dialogue
     */
    public void OnChangeHologramState()
    {
        GetComponent<ObjectInteractionController>().OnChangeDialogue();
    }

}

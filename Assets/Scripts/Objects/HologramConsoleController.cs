using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramConsoleController : ModifiableController, Interactable
{
    [SerializeField] private HologramObjectController hologram;
    public bool canBeInteracted { get; set; }

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.Add("locked", false);
    }
    public void Interact()
    {
        if (TryGet("locked", out bool locked) && hologram)
        {
            if (!locked)
            {
                if (hologram.TryGet("power", out bool power))
                {
                    hologram.SetValue("power", !power);
                    hologram.UpdateModification();
                    hologram.UpdateFile();

                    hologram.OnChangeHologramState();
                }
            }
        }
    }
}

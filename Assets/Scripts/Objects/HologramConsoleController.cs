using UnityEngine;

public class HologramConsoleController : ModifiableController, Interactable
{
    [SerializeField] private HologramObjectController hologram;
    public bool canBeInteracted { get; set; }

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.TryAdd("locked", new DicoValueProperty {IsImportant = true, Value = false});
    }

    public void Interact()
    {
        if (!TryGet("locked", out bool locked) || !hologram) return;
        if (locked) return;
        if (!hologram.TryGet("power", out bool power)) return;
        
        hologram.SetValue("power", !power);
        hologram.OnChangeUpdateAll();
    }
}

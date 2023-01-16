using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivController : ItemController
{
    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.TryAdd("detonate", new DicoValueProperty { IsImportant = true, Value = true });
    }

    public override void UpdateModification()
    {
        //todo : player pref
        print("miaou");
        base.UpdateModification();
        if (!TryGet("detonate", out bool detonate)) return;
        print("DETONATE");     
    }
}

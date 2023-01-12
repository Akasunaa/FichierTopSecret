using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivController : ItemController
{
    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.Add("detonate", new DicoValueProperty { IsImportant = true, Value = false });
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        //For the lamp object, we test if its power is on or off
        if (TryGet("detonate", out bool detonate))
        {
            print("DETONATE");
        }
    }
}

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
        
        //
        print(gameObject.GetComponent<FileParser>().filePath);
        base.UpdateModification();
        if (!TryGet("detonate", out bool detonate)) return;
        if (!detonate) return;
        print("DETONATE");

        string path = gameObject.GetComponent<FileParser>().filePath;
        PlayerPrefs.SetString("HasDetonated", path);
        PlayerPrefs.Save();
    }
}

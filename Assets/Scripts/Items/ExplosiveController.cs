using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosiveController : ItemController
{
    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.TryAdd("detonate", new DicoValueProperty { IsImportant = true, Value = false });
    }

    public override void UpdateModification()
    {
        base.UpdateModification();
        if (!TryGet("detonate", out bool detonate)) return;
        if (!detonate) return;
        print("EXPLOSIVES : DETONATE");

        string path = gameObject.GetComponent<FileParser>().filePath;
        //case one : player launch detonate while explosives is in his scene or in hos inventory : dead
        if (path.Contains("player") || FilesWatcher.IsPathToScene(path, SceneManager.GetActiveScene().name))
        {
            print("EXPLOSIVES : YOU DEAD");
            return;
        }

        //case two : player launch detonate in another scene
        PlayerPrefs.SetString("HasDetonated", path);
        print("EXPLOSIVES : SAVING HAS DETONATED DATA");
        PlayerPrefs.Save();
    }
}

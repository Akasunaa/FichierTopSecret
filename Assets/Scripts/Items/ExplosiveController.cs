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

        string absolutePath = gameObject.GetComponent<FileParser>().filePath;
        string relativePath = Utils.RelativePath(absolutePath);
        //case one : player launch detonate while explosives is in his scene or in hos inventory : dead
        if (Utils.SceneName(relativePath) == "player" || SceneManager.GetActiveScene().name == Utils.SceneName(Utils.RelativePath(absolutePath)))// || Utils.IsPathToScene(path, SceneManager.GetActiveScene().name))
        {
            print("EXPLOSIVES : YOU DEAD");
            //We trigger death here
            //we recuperate the ui :
            GameObject ui = GameObject.FindGameObjectWithTag("UI");
            //we get the correcte component :
            //TODO
            //we launch the right function :
            //TODO
            return;
        }

        //case two : player launch detonate in another scene
        PlayerPrefs.SetString("HasDetonated", absolutePath);
        PlayerPrefs.Save();
    }
}

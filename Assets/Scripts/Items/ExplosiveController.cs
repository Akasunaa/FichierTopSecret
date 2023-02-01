using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script used by the explosives of the player
/// </summary>
public class ExplosiveController : ItemController
{
    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

    public override void SetDefaultProperties()
    {
        base.SetDefaultProperties();
        properties.TryAdd("detonate", new DicoValueProperty { IsImportant = true, Value = false });
    }

    public override void UpdateModification(bool firstRead = false)
    {
        base.UpdateModification();
        if (!TryGet("detonate", out bool detonate)) return;
        if (!detonate) return;

        string absolutePath = gameObject.GetComponent<FileParser>().filePath;
        string relativePath = Utils.RelativePath(absolutePath);
        string sceneName = Utils.SceneName(relativePath);

        Debug.Log("[EXPLOSIVES] DETONATE in " + sceneName);

        //explosion sprite :
        GameObject explosionInstance = Instantiate(explosionEffect);
        explosionInstance.transform.parent = null;
        explosionInstance.transform.position = transform.position;

        //case one : player launch detonate while explosives is in his scene or in hos inventory : dead
        if (sceneName == Utils.PlayerFolderName || (Time.timeSinceLevelLoad > 2f && LevelManager.Capitalize(SceneManager.GetActiveScene().name) == sceneName)) //TODO: c'est moche mais ok tier
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null && playerObject.TryGetComponent(out PlayerObjectController playerObjectController))
            {
                if (playerObjectController.TryGet("health", out int health))
                {
                    if (health > 1000)
                    {
                        playerObjectController.SetValue("health", health - 1000);
                        if (playerObject.TryGetComponent(out FileParser playerFp))
                        {
                            playerFp.WriteToFile();
                        }
                        
                        if (sceneName == "Factoryroom1") //TODO : HARD CODE MAIS PAS GRAVE
                        {
                            PlayerPrefs.SetInt("HasDetonated", 1);
                            PlayerPrefs.Save();
                            GameObject breakableWall = GameObject.FindGameObjectWithTag("BreakableWall");
                            if (breakableWall)
                            {
                                breakableWall.GetComponent<BreakableWallController>().DestroyWall();
                            }
                        }

                        DirectoryInfo di1 = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + sceneName);
                        foreach (FileInfo file in di1.GetFiles())
                        {
                            if (file.Name != "player.txt" || sceneName != Utils.PlayerFolderName)
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(e);
                                }
                            }
                        }
                        foreach (DirectoryInfo dir in di1.GetDirectories())
                        {
                            try
                            {
                                dir.Delete(true);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                        }
                        
                        return;
                    }
                }
            }

            Debug.Log("EXPLOSIVES : YOU DEAD");
            //We trigger death here
            //we recuperate the ui :
            GameObject ui = GameObject.FindGameObjectWithTag("UI");
            if (ui == null) return;

            //we get the correcte component :
            GameOverScreenController gameOverScreenController = ui.GetComponent<GameOverScreenController>();
            if (gameOverScreenController == null) return; 
            
            //we launch the right function :
            gameOverScreenController.OnGameOver(GameOverScreenController.GameOverType.ExplosiveDeath);
            
            return;
        }

        //case two : player launch detonate in another scene
        if (sceneName == "Factoryroom1") //TODO : HARD CODE MAIS PAS GRAVE
        {
            PlayerPrefs.SetInt("HasDetonated", 1);
            PlayerPrefs.Save();
            GameObject breakableWall = GameObject.FindGameObjectWithTag("BreakableWall");
            if (breakableWall)
            {
                breakableWall.GetComponent<BreakableWallController>().DestroyWall();
            }
        }
        
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + sceneName);
        foreach (FileInfo file in di.GetFiles())
        {
            try
            {
                file.Delete();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            try
            {
                dir.Delete(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    private void OnDestroy()
    {
        //GameObject explosionInstance = Instantiate(explosionEffect);
        //explosionInstance.transform.parent = null;
        //explosionInstance.transform.position= transform.position;
    }
}

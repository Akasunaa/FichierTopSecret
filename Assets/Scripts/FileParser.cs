using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using System.Linq;

/**
 *  Component used to handle the read and modify aspects of the game using the file explorer
 *  For now, will try to read a single value and change the appropriate prefab using that
 */
[RequireComponent(typeof(ModifiableController))]
public class FileParser : MonoBehaviour
{
    private string[] _dataArray; //data read from the file
    public string filePath { get; set; } //informations to access file
    [Header("Object File infos")]
    [SerializeField] private string targetObjectFileName;           // ONLY USED FOR THE EDITOR SHOULD NOT BE USED
    public ModifiableController targetModifiable { get; private set; }                  //Modifiable controller of the targetObject that will be called upon identification of the modification and/or other actions

    private void Awake()
    {
        filePath = Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + targetObjectFileName;
        targetModifiable = GetComponent<ModifiableController>();
        Assert.IsNotNull(targetModifiable);
    }

    /**
     *  Function called by FileWatcher upon detection of a file modification
     */
    public bool OnChange(string path)
    {
        ReadFromFile(filePath);
        return true;
    }

     /**
     *  Function called by FileWatcher
     */
    public bool OnDelete(string path)
    {
        if (targetModifiable)
        {
            if (targetModifiable.canBeDeleted)
            {

                if (gameObject.TryGetComponent(out PlayerMovement _)) {
                    WriteToFile();
                    if (LevelManager.Capitalize(SceneManager.GetActiveScene().name) != Utils.CosmicbinFolderName) {
                        LevelManager.Instance.LoadScene(Utils.CosmicbinFolderName); 
                    }
                    return false;
                } //delete player
                gameObject.SetActive(false);
                if(gameObject.TryGetComponent(out ItemController ic)) {
                    Destroy(targetModifiable.gameObject);
                    return true; //item dont go in cosmic bin
                } 
                DeleteFile(path);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerObjectController>().DeleteSound();
                return true;
            }
            // If the file cannot be deleted, re-write the file
            WriteToFile();
            return false;
        }
        return false;

    }

    /**
    * Send Object to cosmic bin
    */
    private void DeleteFile(string path)
    {
        // create CosmicBin if it doesn't exist
        CosmicBinManager.Instance.GenerateCosmicBin();

        // create fileinfo
        var fi = new FileInfo(Application.streamingAssetsPath + path);
        
        // add origin scene as property
        if (Utils.SceneName(fi) != Utils.CosmicbinFolderName)
        {
            targetModifiable.SetValue("scene target", LevelManager.Capitalize(SceneManager.GetActiveScene().name));
            filePath = Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + Utils.CosmicbinFolderName + "/" + fi.Name;
            WriteToFile();
            File.SetAttributes(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + Utils.CosmicbinFolderName + "/" + Utils.FileName(Utils.RelativePath(filePath)), FileAttributes.ReadOnly);
            //if (TryGetComponent(out ModifiableController mc)) { 
            //    mc.canBeDeleted = false; 
            //}
        }
        //put particle 
        ParticleSystem particles = Instantiate(FindObjectOfType<LevelManager>().depopParticle);
        particles.gameObject.transform.position = transform.position;
        particles.Play();
        Destroy(particles.gameObject, 1);
    }

    /**
     *  Function that will analyse the file found at filePath and will obtain the value needed (targetObjectModifiedVariable)
     *  called from ???
     * firstRead : is true only after the scene is loaded
     */
    public void ReadFromFile(string path, bool firstRead = false)
    {
        Char[] separators = {':', '='};
        _dataArray = File.ReadAllLines(path);
        if(_dataArray != null)
        {
            var keyNames = new List<string>();
            foreach (var line in _dataArray)
            {
                if (separators.Any(sep => line.Contains(sep)))
                {
                    var lineSplit = line.Split(separators);
                    var keyName = lineSplit[0];
                    var value = string.Join("", lineSplit[1..]);
                    targetModifiable.OnModification(keyName.Trim().ToLower(), value.Trim()); // modifying appropriate variable
                    keyNames.Add(keyName);
                }
            }

            if (targetModifiable.UpdatePropertiesDico(keyNames))
            {
                WriteToFile();
            }
        }
        else
        {
            Debug.LogWarning("File.ReadAllLines("+path+") is null");
        }

        targetModifiable.UpdateModification(firstRead);
    }

    public void WriteToFile()
    {

        Debug.Log(name + " write to file " + filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        if (File.Exists(filePath))
        {
            var fileIsReadonly = (File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0;
            if (fileIsReadonly) return;
        }

        using (var sw = new StreamWriter(filePath))  
        {  
            sw.Write(targetModifiable.ToFileString());
        }
    }
}

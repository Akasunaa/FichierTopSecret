using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Assertions;

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
    [SerializeField] private string targetObjectFileName;           //file (name) where the modification is
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
        if (targetModifiable) //Ceci est vraiment a corriger au plus vite
        {
            if (targetModifiable.canBeDeleted)
            {
                gameObject.SetActive(false);
                DeleteFile(path);

                return true;
            }
            // If the file cannot be deleted, re-write the file
            WriteToFile();
            return false;
        }
        return false;

    }

    public void DeleteFile(string path)
    {
        // create CosmicBin if it doesn't exist
        CosmicBinManager.Instance.GenerateCosmicBin();

        // create fileinfo
        var fi = new FileInfo(Application.streamingAssetsPath + path);

        // add origin scene as property
        if (!fi.FullName.Contains("Cosmicbin"))
        {
            targetModifiable.SetValue("scene target", SceneManager.GetActiveScene().name);
            filePath = Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/Cosmicbin/" + fi.Name;
            WriteToFile();
            File.SetAttributes(Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/Cosmicbin/" + targetObjectFileName.Split("/")[^1], FileAttributes.ReadOnly);
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
     */
    public void ReadFromFile(string path)
    {
        const string separator = ":";
        _dataArray = File.ReadAllLines(path);
        if(_dataArray != null)
        {
            var keyNames = new List<string>();
            var test = false;
            foreach (var line in _dataArray)
            {
                if (line.Contains(separator))
                {
                    var lineSplit = line.Split(separator);
                    var keyName = lineSplit[0];
                    var value = string.Join("", lineSplit[1..]);
                    test = targetModifiable.OnModification(keyName.Trim().ToLower(), value.Trim()) || test; // modifying appropriate variable
                    keyNames.Add(keyName);
                }
            }
            test = targetModifiable.UpdatePropertiesDico(keyNames) || test;
            if(test) WriteToFile();
        }
        else
        {
            Debug.LogWarning("File.ReadAllLines("+path+") is null");
        }

        targetModifiable.UpdateModification();
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

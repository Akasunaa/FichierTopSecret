using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using Mono.Cecil.Rocks;
[RequireComponent(typeof(ModifiableController))]

/**
 *  Component used to handle the read and modify aspects of the game using the file explorer
 *  For now, will try to read a single value and change the appropriate prefab using that
 */
public class FileParser : MonoBehaviour
{
    private string[] dataArray; //data read from the file
    public string filePath { get; set; } //informations to access file
    [Header("Object File infos")]
    [SerializeField] private string targetObjectFileName;           //file (name) where the modification is
    public ModifiableController targetModifiable { get; private set; }                  //Modifiable controller of the targetObject that will be called upon identification of the modification and/or other actions

    void Awake()
    {
        if (String.IsNullOrEmpty(targetObjectFileName))
        {
            Debug.LogError("targetObjectFileName in " + name + "cannot be null !");
            #if UNITY_EDITOR
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPaused = true;
            }
            #endif
            throw new ArgumentNullException("targetObjectFileName cannot be null !");
        }
        filePath = Application.streamingAssetsPath + "/Test" + "/" + targetObjectFileName;
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
        if (targetModifiable.canBeDeleted)
        {
            Destroy(targetModifiable.gameObject);
            //todo : moyen de le recreer si unique
            return true;
        }
        FileInfo fileInfo = new FileInfo(filePath);
        using (StreamWriter sw = new StreamWriter(fileInfo.FullName))
        {
            sw.Write(targetModifiable.ToFileString());
            //todo : completer selon ce que le prefabs contient 
        }
        return true;
    }

    /**
     *  Function that will analyse the file found at filePath and will obtain the value needed (targetObjectModifiedVariable)
     *  called from ???
     */
    public void ReadFromFile(string path)
    {
        const string separator = ":";
        dataArray = File.ReadAllLines(path);
        if(dataArray != null)
        {
            foreach (string line in dataArray)
            {
                if (line.Contains(separator))
                {
                    var lineSplit = line.Split(separator);
                    string name = lineSplit[0];
                    string value = string.Join("", lineSplit[1..]);
                    targetModifiable.OnModification(name.Trim().ToLower(), value.Trim()/*.ToLower()*/); // modifiying appropriate variable
                }
            }
        }
       

        targetModifiable.UpdateModification();
    }

    public void WriteToFile()
    {
        using (StreamWriter sw = new StreamWriter(filePath))  
        {  
            sw.Write(targetModifiable.ToFileString());
        }
    }
}

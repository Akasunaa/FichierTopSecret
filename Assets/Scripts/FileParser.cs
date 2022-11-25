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
        if (targetModifiable) //Ceci est vraiment a corriger au plus vite
        {
            if (targetModifiable.canBeDeleted)
            {
                Debug.Log("Deleting go : " + path + " | " + name);
                Destroy(targetModifiable.gameObject);
                //todo : moyen de le recreer si unique
                return true;
            }
            // If the file cannot be deleted, re-write the file
            WriteToFile();
            return false;
        }
        return false;

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
                    string keyName = lineSplit[0];
                    string value = string.Join("", lineSplit[1..]);
                    targetModifiable.OnModification(keyName.Trim().ToLower(), value.Trim()); // modifying appropriate variable
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

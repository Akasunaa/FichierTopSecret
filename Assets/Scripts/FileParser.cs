using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;

/**
 *  Component used to handle the read and modify aspects of the game using the file explorer
 *  For now, will try to read a single value and change the appropriate prefab using that
 */
public class FileParser : MonoBehaviour
{
    private string[] dataArray; //data read from the file
    private string filePath; //informations to access file
    [Header("Object File infos")]
    [SerializeField] private GameObject targetObject;               //object that will take the modification searched for
    [SerializeField] private string targetObjectFileName;           //file (name) where the modification is
    [SerializeField] private string targetObjectModifiedVariable;   //variable that will be looked for and modified
    private ModifiableController targetModifiable;                  //Modifiable controller of the targetObject that will be called upon identification of the modification and/or other actions

    private void Start()
    {
        filePath = Application.streamingAssetsPath + "/" + targetObjectFileName;
        targetModifiable = targetObject.GetComponent<ModifiableController>();
        Assert.IsNotNull(targetModifiable);
    }

    private void Update() //FOR NOW, SHOULD BE DELETED LATER with check being done by calls from FileWatcher rather than every frame
    {
        ReadFromFile();
    }

    /**
     *  Function called by FileWatcher upon detection of a file modification
     */
    public bool OnChange(FileInfo fileInfo)
    {
        return true;
    }

    /**
     *  Function that will analyse the file found at filePath and will obtain the value needed (targetObjectModifiedVariable)
     *  called from ???
     */
    public void ReadFromFile()
    {
        dataArray = File.ReadAllLines(filePath);
        foreach(string line in dataArray)
        {
            if(line.Contains(targetObjectModifiedVariable + " : ")) //FOR NOW IT ONLY SCANS FOR A SINGLE VARIABLE -> SHOULD BE A LIST LATER TO CHECK FOR ALL RELEVANT VARIABLES
            {
                string value = line.Split(" : ")[1];                                    //obtaining value (modified or not)
                targetModifiable.OnModification(targetObjectModifiedVariable, value);   //modifiying appropriate variable
                return; //FOR NOW, ONCE THE CORRECT VAR IS FOUND, WE QUIT THE SEARCH AFTERWARDS (since we search only 1 var)
            }
        }
    }
}

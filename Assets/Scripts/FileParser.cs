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

    /**
     *  Function called by FileWatcher
     */
    public bool OnChange(FileInfo fileInfo)
    {
        return true;
    }

    /**
     *  Function that will analyse the file found at filePath and will obtain the value needed (targetObjectModifiedVariable)
     */
    public void ReadFromFile()
    {
        dataArray = File.ReadAllLines(filePath);
        foreach(string line in dataArray)
        {
            if(line.Contains(targetObjectModifiedVariable + " :")) //FOR NOW IT ONLY SCANS FOR A SINGLE VARIABLE -> SHOULD BE A LIST LATER TO CHECK FOR ALL RELEVANT VARIABLES
            {
                string value = line.Split(" : ")[1];
                //IMPLEMENT VALUE TO CORRESPONDING VARIABLE IN SCRIPT OF ENTITY
                targetModifiable.OnModification(targetObjectModifiedVariable, value);
                return;
            }
        }
    }
}

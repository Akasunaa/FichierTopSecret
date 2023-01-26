using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script used by special dialog states that will add a new file .txt in a character's iventory i.e. new folder
/// </summary>
[CreateAssetMenu(fileName = "File Dialog State", menuName = "States/File Dialog State")]
public class FileDialogState : DialogState
{
    [Header("File informations to be added to inventory")]
    [SerializeField] private string fileName;
    [TextArea(3,10)]
    [SerializeField] private string fileData;
    private string filePath = Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/Office/";

    public FileDialogState(string name, DialogSM SM) : base(name, SM)
    {
    }

    /// <summary>
    /// Function that will change the currently displayed speech of the state : at every call of the function by external scripts, will switch to next speech until the last one
    /// Main difference given by that script : 
    /// here, unlike regular dialog states, we're going to modify a file property
    /// </summary>
    /// <returns>0 if the currentSpeech is the last speech, 1 otherwise</returns>
    public override int ChangeSpeech()
    {
        interactionIndex++;
        if (interactionIndex < speech.Length)
        {
            currentSpeech = speech[interactionIndex];
            GetSpeechVariables(SM);
            return 1;
        }
        else //when reaching the end of the various speeches, the NPC will repeat the last inputted speech
        {
            currentSpeech = speech[^1];
            GetSpeechVariables(SM);
            CreateInventoryFile();
            return 0;
        }
    }

    private void CreateInventoryFile()
    {
        FileParser component;
        SM.gameObject.TryGetComponent<FileParser>(out component);
        filePath = component.filePath;
        Debug.Log("DONKEY : " + filePath);
        filePath = Application.streamingAssetsPath + "/" + Utils.RootFolderName + "/" + Utils.SceneName(Utils.RelativePath(filePath))+"/";
        Debug.Log("DONKEY : " + filePath);
        filePath = filePath + SM.gameObject.name+"/";
        Debug.Log("DONKEY : " + filePath);
        Directory.CreateDirectory(filePath);

        try
        {
            using (var sw = new StreamWriter(filePath + "/" + fileName))
            {
                sw.Write(fileData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        //File.SetAttributes(filePath + "/" + fileName, FileAttributes.ReadOnly);

    }
}

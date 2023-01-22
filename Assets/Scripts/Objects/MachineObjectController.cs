using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
[RequireComponent(typeof(FileParser))]

public class MachineObjectController : ModifiableController, Interactable
{
    public bool canBeInteracted { set; get; }
    [SerializeField] private string objectName;

    public override void SetDefaultProperties()
    {
        GetComponent<FileParser>().WriteToFile();
    }

    public override string ToFileString()
    {
        return "set count=0\nfor %%x in (" + objectName + "*.txt) do set /a count+=1\necho NUL > " + objectName + "-%count%.txt";
    }

    void Start()
    {
        canBeInteracted = true;
    }

    public void Interact()
    {
        string filePath = GetComponent<FileParser>().filePath;
        FileInfo fi = new FileInfo(filePath);
        Debug.Log("Interact machine: " + fi.Name + " | " + fi.DirectoryName);
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + filePath);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.WorkingDirectory = fi.DirectoryName;
        
        Process process = Process.Start(processInfo);
        process.WaitForExit();
        process.Close();
    }
}

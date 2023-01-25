using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MachineObjectController : ModifiableController, Interactable
{
    public bool canBeInteracted { set; get; }
    [SerializeField] private string objectName;
    private DialogueUIController uiDialogController;
    [TextArea(3, 10)]
    [SerializeField] private string dialogueNoMoney;
    [TextArea(3, 10)]
    [SerializeField] private string dialogueCooldown;
    private GameObject playerObject;
    private bool isInInteraction = false;
    [SerializeField] private float machineCooldown = 2f;
    private float machineUsageTime = 0;

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
        uiDialogController = GameObject.FindGameObjectWithTag("UI").GetComponent<DialogueUIController>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact()
    {
        if (playerObject != null && Time.time > machineUsageTime + machineCooldown)
        {
            if (playerObject.TryGetComponent(out PlayerObjectController playerObjectController))
            {
                if (playerObjectController.TryGet("money", out int money) && money >= 10)
                {
                    machineUsageTime = Time.time;
                    playerObjectController.SetValue("money", money - 10);
                    if (playerObject.TryGetComponent(out FileParser fp))
                    {
                        fp.WriteToFile();
                    }
                    string filePath = GetComponent<FileParser>().filePath;
                    FileInfo fi = new FileInfo(filePath);
                    ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + filePath);
                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;
                    processInfo.WorkingDirectory = fi.DirectoryName;
        
                    Process process = Process.Start(processInfo);
                    process.WaitForExit();
                    process.Close();
                    return;
                }
            }
        }

        if (!isInInteraction)
        {
            playerObject.GetComponent<InputController>().StopMovement();
            if (Time.time <= machineUsageTime + machineCooldown)
            {
                uiDialogController.DisplayDialogue(dialogueCooldown, "player");
            }
            else
            {
                uiDialogController.DisplayDialogue(dialogueNoMoney, "player");   
            }

            isInInteraction = true;
        }
        else
        {
            uiDialogController.EndDisplay();
            playerObject.GetComponent<InputController>().RestartMovement();
            playerObject.GetComponent<PlayerObjectController>().InteractSound();
            isInInteraction = false;
        }
    }
}

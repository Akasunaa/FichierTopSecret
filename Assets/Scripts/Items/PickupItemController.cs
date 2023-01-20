using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PickupItemController : ModifiableController, Interactable
{
    public bool canBeInteracted { get; set; }

    public void Interact()
    {
        if (TryGetComponent(out FileParser fp))
        {
            FileInfo fi = new FileInfo(fp.filePath);
            if (fi.Exists)
            {
                LevelManager.GiveItem(fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length));
                fi.Delete();
            }
        }
    }
}
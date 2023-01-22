using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerButton : MonoBehaviour
{
    public static void OpenExplorer()
    {
        if (FilesWatcher.instance == null || FilesWatcher.instance.explorerHwnd == IntPtr.Zero)
        {
            Application.OpenURL("file:///" + Application.streamingAssetsPath + "/" + Utils.RootFolderName);
        }
        else
        {
            FilesWatcher.SetForegroundWindow(FilesWatcher.instance.explorerHwnd);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerButton : MonoBehaviour
{
    public static void OpenExplorer()
    {
        Application.OpenURL("file:///" + Application.streamingAssetsPath + "/" + Utils.RootFolderName);
    }
}

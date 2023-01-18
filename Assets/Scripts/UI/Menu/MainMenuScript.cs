using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("SceneLauncher");
    }
    
    public void Settings()
    {
        Debug.Log("GOTO Settings I hope");
        // SceneManager.LoadScene("SceneLauncher");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private MainMenuButton[] buttons;
    private int selectedButton = 0;

    void Start()
    {
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        IntPtr unityWindow = FilesWatcher.GetActiveWindow();
        if (unityWindow != IntPtr.Zero)
        {
            if (FilesWatcher.GetWindowRect(unityWindow, out FilesWatcher.RECT r))
            {
                FilesWatcher.MoveWindow(unityWindow, 0, 50,
                    r.Right - r.Left, r.Bottom - r.Top, true);
            }
        }
        #endif
    }

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

    public void Selected(int n)
    {
        selectedButton = n % buttons.Length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.D))
        {
            buttons[selectedButton].UnSelect();
            selectedButton += 1;
            selectedButton %= buttons.Length;
            buttons[selectedButton].Select();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
        {
            buttons[selectedButton].UnSelect();
            selectedButton -= 1 - buttons.Length;
            selectedButton %= buttons.Length;
            buttons[selectedButton].Select();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            buttons[selectedButton].onClick.Invoke();
        }
    }
}

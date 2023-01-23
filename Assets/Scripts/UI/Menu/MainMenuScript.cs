using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuCanvas;
    [SerializeField] private MainMenuButton[] buttons;
    private int selectedButton = 0;

    private void Awake()
    {
        Assert.IsNotNull(settingsMenuCanvas);
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
        settingsMenuCanvas.SetActive(true);
        gameObject.SetActive(false);
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

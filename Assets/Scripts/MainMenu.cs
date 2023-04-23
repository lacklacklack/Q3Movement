using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button timerModeButton;

    public void LoadStoryMode()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}
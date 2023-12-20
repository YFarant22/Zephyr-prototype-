using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_script : MonoBehaviour
{
    public string LevelToLoad;
    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelToLoad);
    }

    public void quitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false; 
        Application.Quit();
    }
}

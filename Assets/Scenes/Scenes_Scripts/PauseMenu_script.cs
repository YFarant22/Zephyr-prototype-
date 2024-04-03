using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.SceneSearch;

public class PauseMenu_script : MonoBehaviour
{
    public string LevelToLoad;
    public GameObject pauseMenu;
    
    public void Update()
    {
        if (pauseMenu.activeSelf != true && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("pause activated");
            pauseMenu.SetActive(true);
        }
        
        
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
    
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
    }
}

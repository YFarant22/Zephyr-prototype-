using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.SceneSearch;

public class PauseMenu_script : MonoBehaviour
{
    public string LevelToLoad; 
        
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
    
    public void ResumeGame()
    {
        gameObject.SetActive(false);
    }
}

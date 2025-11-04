using System;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneHandler : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Main");  // Tải Scene Game
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("Main Menu");  // Tải Scene Menu
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load our demo level, by its scene index in build settings
        SceneManager.LoadSceneAsync(1);
    }
}

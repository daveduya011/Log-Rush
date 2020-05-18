using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void GoToScene(int index) {
        SceneManager.LoadScene(index);
    }
    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void Exit() {
        Application.Quit();
    }
    public void PlaySinglePlayer() {
        SceneManager.LoadScene("MainGame");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public RectTransform splashPanel;
    public void btnPlay()
    {
        splashPanel.gameObject.SetActive(true);
        splashPanel.GetComponent<Animator>().SetBool("isSplashIn", true);
        Invoke("Delay", 0.7f);
    }

    public void Delay()
    {
        SceneManager.LoadScene("MainGame");
    }
}

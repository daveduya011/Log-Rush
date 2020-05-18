using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerScoreManager : MonoBehaviour
{
    public TextMeshProUGUI winText;
    void Start()
    {
        PhotonNetwork.Disconnect();
        if (GameManager.Instance.isWin) {
            winText.text = "won!";
        } else {
            winText.text = "lose!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

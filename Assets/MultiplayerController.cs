using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MultiplayerController : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    public CameraController cameraController;
    public Transform spawnPosition;
    public LogManager logManager;
    public RectTransform instructionsPanel;
    public float timer = 5;
    public float tempTimer;


    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.NickName = SaveSystem.LoadPlayer().username;
        PV = GetComponent<PhotonView>();
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Character"), spawnPosition.position, Quaternion.identity);
        logManager.isMultiplayer = true;
        instructionsPanel.gameObject.SetActive(true);
        tempTimer = timer;
        logManager.finishLine = -1;
        if (PhotonNetwork.IsMasterClient) {
            // Generate finish line
            int rand = RandomGenerator.GenerateFinishLine();
            PV.RPC("RPC_SendFinishLine", RpcTarget.AllBuffered, rand);
        }
    }

    void Start() {
        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(CountdownTime());
        }
    }

    private IEnumerator CountdownTime() {
        while (tempTimer > 0) {
            tempTimer--;
            PV.RPC("RPC_SendTimer", RpcTarget.All, tempTimer);
            yield return new WaitForSeconds(1f);
        } 
        if (tempTimer == 0) {
            PV.RPC("RPC_SendStartGame", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void RPC_SendTimer(float time) {
        logManager.score.PlayText(string.Format("{0:00}", time));
    }
    [PunRPC]
    public void RPC_SendStartGame() {
        logManager.isStarted = true;
        instructionsPanel.gameObject.SetActive(false);
    }
    [PunRPC]
    public void RPC_SendFinishLine(int index) {
        logManager.finishLine = index;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("PlayerLeft");
        logManager.player.SetFinishLineReached();
    }

}

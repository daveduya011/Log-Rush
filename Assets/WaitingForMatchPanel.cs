using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForMatchPanel : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    public Text statusText;
    public Button button;
    public Image loadingImage;
    public LoadingPanel loadingPanel;

    public bool isMatchFound;
    public bool isEnemiesReady;
    public bool isGameStarted;

    public int minPlayersToStart = 2;
    public float countdownTime = 5f;
    public float tempCountdownTime;
    public int lastTempCountdownTime;

    private RoomProfile myRoomProfile;
    public void Show() {
        gameObject.SetActive(true);

        myRoomProfile = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Profile"), Vector3.zero, Quaternion.identity).GetComponent<RoomProfile>();
    }
    public override void OnDisable() {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);

        button.onClick.RemoveAllListeners();
    }
    public override void OnEnable() {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        button.onClick.AddListener(() => OnClick());
        PV = GetComponent<PhotonView>();

        UpdateMatches();
    }

    private void ResetAll() {
        // On gameobject has been enabled, then set default values
        button.interactable = true;
        tempCountdownTime = countdownTime;
        lastTempCountdownTime = 0;
        isEnemiesReady = false;
        isMatchFound = false;
        isGameStarted = false;
    }

    public void Update() {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (isEnemiesReady) {
            if (tempCountdownTime >= 0) {
                tempCountdownTime -= Time.deltaTime;
                // send RPC only by seconds
                if (lastTempCountdownTime != (int)tempCountdownTime) {
                    PV.RPC("RPC_SendRoomTimer", RpcTarget.All, tempCountdownTime);
                }
                lastTempCountdownTime = (int)tempCountdownTime;
            }
            else {
                if (!isGameStarted) {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    StartCoroutine(LoadLevelAsync());
                    isGameStarted = true;
                }
            }
        }
    }

    private IEnumerator LoadLevelAsync() {
        loadingPanel.Show();
        PV.RPC("RPC_SendLoading", RpcTarget.All, PhotonNetwork.LevelLoadingProgress);

        PhotonNetwork.LoadLevel("BattleGame");

        while (PhotonNetwork.LevelLoadingProgress < 0.9f) {
            PV.RPC("RPC_SendLoading", RpcTarget.All, PhotonNetwork.LevelLoadingProgress);
            loadingPanel.SetProgress(PhotonNetwork.LevelLoadingProgress + 0.1f);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnClick() {
        if (!isMatchFound) {
            Close();
        } else {
            if (myRoomProfile.isReady) {
                // cancel ready
                Close();
            } else {
                SetReady();
            }
        }
    }

    private void SetReady() {
        myRoomProfile.GetComponent<RoomProfile>().SetReady(true);
        button.GetComponentInChildren<Text>().text = "cancel";
        PV.RPC("RPC_NotifyReady", RpcTarget.All);
    }

    [PunRPC]
    public async void RPC_NotifyReady() {
        bool isBothPlayersReady = CheckBothPlayersReady();

        while (!isBothPlayersReady) {
            await Task.Delay(500);
            isBothPlayersReady = CheckBothPlayersReady();
        }
        isEnemiesReady = true;
    }

    private bool CheckBothPlayersReady() {

        foreach (var player in PhotonNetwork.PlayerList) {
            RoomProfile roomProfile = ((GameObject)player.TagObject).GetComponent<RoomProfile>();
            if (!roomProfile.isReady) {
                return false;
            }
        }

        return true;
    }

    [PunRPC]
    public void RPC_SendRoomTimer(float time) {
        button.interactable = false;
        statusText.text = string.Format("Match will start in {0:00}", time);
    }

    [PunRPC]
    public void RPC_SendLoading(float progress) {
        loadingPanel.Show();
        loadingPanel.SetProgress(PhotonNetwork.LevelLoadingProgress + 0.1f);
    }

    private void UpdateMatches() {
        ResetAll();
        int playerCount = PhotonNetwork.PlayerList.Length;
        if (playerCount == minPlayersToStart) {
            loadingImage.gameObject.SetActive(false);
            statusText.text = "match found!";
            isMatchFound = true;
            button.GetComponentInChildren<Text>().text = "ready";
        }
        else {
            loadingImage.gameObject.SetActive(true);
            statusText.text = "waiting for a match";
            button.GetComponentInChildren<Text>().text = "cancel";
            isMatchFound = false;
            isEnemiesReady = false;
        }
    }

    public void Close() {
        PhotonNetwork.LeaveRoom();
        gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        UpdateMatches();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        myRoomProfile.SetReady(false);
        UpdateMatches();
    }
}
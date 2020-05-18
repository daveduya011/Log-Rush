using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public MatchManager matchManager;
    public int numOfRetry = 5;
    private int tempNumOfRetry;

    // Start is called before the first frame update
    void Start()
    {
        tempNumOfRetry = numOfRetry;
    }

    private void OnLoginAction(OnlineDatabase.ReturnMessage arg1, string arg2) {
        matchManager.OnLoginAction(arg1, arg2);
    }

    public override void OnConnectedToMaster() {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public async void FindMatch() {
        Debug.Log("Server: " + PhotonNetwork.NetworkingClient.Server);
        OnLoginAction(OnlineDatabase.ReturnMessage.PENDING, "creating match");
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.NetworkingClient.Server == ServerConnection.MasterServer) {
            PhotonNetwork.JoinRandomRoom();
            tempNumOfRetry = numOfRetry;
        }
        // if not connected, then display check internet connection
        else {
            OnLoginAction(OnlineDatabase.ReturnMessage.PENDING, "creating match");
            Debug.Log("NumOfTries " + tempNumOfRetry);
            await Task.Delay(1500);
            if (tempNumOfRetry != 0) {
                PhotonNetwork.LeaveRoom();
                FindMatch();
                tempNumOfRetry--;
            } else {
                tempNumOfRetry = numOfRetry;
                OnLoginAction(OnlineDatabase.ReturnMessage.LOGGED_IN, "check internet connection");
            }
        }
    }

    public override void OnJoinedRoom() {
        matchManager.waitingForMatchPanel.Show();
        OnLoginAction(OnlineDatabase.ReturnMessage.LOGGED_IN, "creating match");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        // on failed to join room, try to create a room
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        CreateRoom();
    }
    public override void OnDisconnected(DisconnectCause cause) {
        base.OnDisconnected(cause);
        matchManager.waitingForMatchPanel.Close();
    }
    private void CreateRoom() {
        RoomOptions roomOptions = new RoomOptions {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
            
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

}

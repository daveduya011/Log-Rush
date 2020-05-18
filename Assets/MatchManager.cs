
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{

    public Button button;
    public UsernameInput usernameInput;
    public PlayerInfoPanel playerInfoPanel;
    public NetworkController networkController;
    public WaitingForMatchPanel waitingForMatchPanel;

    private string buttonMessage;

    void Start() {
    }

    private void PlayMultiplayer(PlayerData playerData) {
        if (!PhotonNetwork.IsConnectedAndReady) {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        playerInfoPanel.ReloadAll();
        // If connected, then join a random room
        networkController.FindMatch();
    }


    public void OnLoginAction(OnlineDatabase.ReturnMessage response, string message) {
        button.GetComponentInChildren<Text>().text = message;
        if (response == OnlineDatabase.ReturnMessage.PENDING) {
            button.interactable = false;
        } else if (response == OnlineDatabase.ReturnMessage.LOGGED_IN) {
            buttonMessage = "battle";
            Invoke("RevertButton", 1.5f);
        }
        else {
            buttonMessage = "retry";
            Invoke("RevertButton", 1.5f);
        }
    }

    private void RevertButton() {
        button.interactable = true;
        button.GetComponentInChildren<Text>().text = buttonMessage;
    }


    public void OnClickBattle() {

        PlayerData playerData = SaveSystem.LoadPlayer();
        OnLoginAction(OnlineDatabase.ReturnMessage.PENDING, "loading");

        // if username saved on local is null, then check online
        if (string.IsNullOrEmpty(playerData.username)) {
            if (!OnlineDatabase.Instance.isConnected) {
                OnlineDatabase.Instance.RequestOnlineAccess();
            }
            else {
                string id = OnlineDatabase.Instance.userId;
                string idToken = OnlineDatabase.Instance.idToken;
                FirebaseConfig.LoadUserData(id, idToken).Then(data => {
                    OnLoginAction(OnlineDatabase.ReturnMessage.PENDING, "checking user");
                    if (IsNewUser(data)) {
                        RegisterNewUser(id, idToken);
                    }
                    else {
                        // save the data retrieved from the server
                        SaveSystem.SavePlayer(data);
                        PlayMultiplayer(data);
                    }
                }).Catch(e => {
                    OnlineDatabase.Instance.RequestOnlineAccess();
                    OnLoginAction(OnlineDatabase.ReturnMessage.FAILED, "failed");
                });
            }
        } else {
            // save the current data to server
            if (OnlineDatabase.Instance.isConnected) {
                string id = OnlineDatabase.Instance.userId;
                string idToken = OnlineDatabase.Instance.idToken;
                FirebaseConfig.SaveUserData(id, idToken, playerData);
            }
            PlayMultiplayer(playerData);
        }
    }


    private async void RegisterNewUser(string id, string idToken) {
        string name = await usernameInput.RequestUsernameInput(id, idToken);
        bool isExists = await OnlineDatabase.Instance.CheckIfUsernameExists(name);

        if (isExists) {
            RegisterNewUser(id, idToken);
        } else {
            // Save player data
            PlayerData data = SaveSystem.LoadPlayer();
            data.username = name;

            FirebaseConfig.SaveUserData(id, idToken, data).Then(response => {
                PlayMultiplayer(data);
            }).Catch(e => {
                OnLoginAction(OnlineDatabase.ReturnMessage.FAILED, "Failed to register");
            });
        }
    }

    private bool IsNewUser(PlayerData data) {
        return string.IsNullOrEmpty(data.username);
    }
    void OnEnable() {
        // If player is not yet logged in, then register listeners
        if (string.IsNullOrEmpty(SaveSystem.LoadPlayer().username)) {
            OnlineDatabase.OnLoginAction += OnLoginAction;
        }
    }
    void OnDisable() {
        //Unregister
        if (string.IsNullOrEmpty(SaveSystem.LoadPlayer().username)) {
            OnlineDatabase.OnLoginAction -= OnLoginAction;
        }
    }
}

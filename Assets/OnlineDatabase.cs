using Facebook.Unity;
using FullSerializer;
using Photon.Pun;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OnlineDatabase : MonoBehaviour
{
    public enum ReturnMessage
    {
        LOGGED_IN, PENDING, FAILED
    }
    public static event Action<ReturnMessage, string> OnLoginAction = delegate { };
    
    public bool isConnected;

    public string idToken;
    public string userId;
    private string refreshToken;


    private static OnlineDatabase _instance;

    public static OnlineDatabase Instance
    {
        get {
            if (_instance != null)
                return _instance;
            var go = new GameObject { name = nameof(OnlineDatabase) };
            _instance = go.AddComponent<OnlineDatabase>();
            DontDestroyOnLoad(go);
            return _instance;
        }
    }

    public void Awake() {
        if (_instance != null)
            return;
        else {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(() => OnActivate(), isGameShown => {
                if (!isGameShown) {
                    // Pause the game - we will need to hide
                    Time.timeScale = 0;
                }
                else {
                    // Resume the game - we're getting focus again
                    Time.timeScale = 1;
                }
            });
        }
        else {
            // Already initialized, signal an app activation App Event
            OnActivate();
        }
    }

    private void OnActivate() {
        FB.ActivateApp();

        // If player has accepted online login, then automatically request for online access
        PlayerData data = SaveSystem.LoadPlayer();
        if (!string.IsNullOrEmpty(data.username)) {
            RequestOnlineAccess();
        }
    }

    public void RequestOnlineAccess() {
        var perms = new List<string>() { "public_profile" };
        OnLoginAction(ReturnMessage.PENDING, "logging in");
        string accessToken = AccessToken.CurrentAccessToken != null ? AccessToken.CurrentAccessToken.TokenString : "";

        if (!FB.IsLoggedIn) {
            FB.LogInWithReadPermissions(perms, result => {
                if (AccessToken.CurrentAccessToken != null)
                    accessToken = AccessToken.CurrentAccessToken.TokenString;
                RequestSignIn(accessToken).Catch(e => {
                    OnLoginAction(ReturnMessage.FAILED, "failed to login");
                });
            });
        }
        else {
            RequestSignIn(accessToken).Catch(e => {
                OnLoginAction(ReturnMessage.FAILED, "failed to login");
            });
        }
    }

    private RSG.IPromise<SignResponse> RequestSignIn(string accessToken) {
        var response = FirebaseConfig.SignInToken(accessToken);
        isConnected = true;

        response.Then(data => {

            userId = data.localId;
            refreshToken = data.refreshToken;
            idToken = data.idToken;

            FirebaseConfig.SetLastUserLogin(userId, idToken).Then(res2 => {
                OnLoginAction(ReturnMessage.LOGGED_IN, "login success!");
            }).Catch(e => {
                OnLoginAction(ReturnMessage.FAILED, "failed");
            });
            StartCoroutine(StartRefreshCountDown(refreshToken, float.Parse(data.expiresIn)));
        }).Catch(error => {
            OnLoginAction(ReturnMessage.FAILED, "check internet connection");
        });

        return response;
    }


    public IEnumerator StartRefreshCountDown(string refreshToken, float time) {
        // wait 5 minutes before the token expiry, refresh the token again
        yield return new WaitForSecondsRealtime(time - 300f);

        FirebaseConfig.RefreshToken(refreshToken)
            .Then(response => {
                StartCoroutine(StartRefreshCountDown(refreshToken, float.Parse(response.expires_in)));
            }).Catch(e => {
                Debug.Log("Token Refresh failed" + e.Message);
            });
    }

    public async Task<bool> CheckIfUsernameExists(string username) {

        var users = await FirebaseConfig.GetUsers();
        foreach (var user in users.Values) {
            Debug.Log(user.username);
            if (user.username.ToLower() == username.ToLower()) {
                return true;
            }
        }
        return false;
    }
}




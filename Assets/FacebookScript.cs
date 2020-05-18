//using Facebook.Unity;
//using Proyecto26;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class FacebookScript : MonoBehaviour
//{
//    public RectTransform panelEnterName;

//    // Start is called before the first frame update
//    void Awake()
//    {
//        if (!FB.IsInitialized) {
//            // Initialize the Facebook SDK
//            FB.Init(FBInitCallback, OnHideUnity);
//        }
//        else {
//            // Already initialized, signal an app activation App Event
//            FB.ActivateApp();
//        }
//    }

//    private void FBInitCallback() {
//        if (userId == null) {
//            // Signal an app activation App Event
//            FB.ActivateApp();
//            SignInAction(true, "Facebook SDK successfully initialized");

//            // Continue with Facebook SDK
//            // ...
//        }
//        else {
//            SignInAction(false, "Failed to Initialize the Facebook SDK");
//        }
//    }

//    private void SignInAction(bool success, string error) {
//        PrintToConsole(error);
//    }

//    private void PrintToConsole(string text) {
//        consoleText.text += "\n" + text;
//    }

//    private void OnHideUnity(bool isGameShown) {
//        if (!isGameShown) {
//            // Pause the game - we will need to hide
//            Time.timeScale = 0;
//        }
//        else {
//            // Resume the game - we're getting focus again
//            Time.timeScale = 1;
//        }
//    }

//    public void LoginWithFacebook() {
//        var perms = new List<string>() { "id", "user_friends" };
//        if (!FB.IsLoggedIn) {
//            FB.LogInWithReadPermissions(perms, OnFacebookLoginResult);
//        } else {
//            if (userId != null) {
//                RefreshToken();
//            }
//        }
//    }
//    private void OnFacebookLoginResult(ILoginResult result) {
//        if (FB.IsLoggedIn) {
//            AccessToken token = AccessToken.CurrentAccessToken;
//            SignInAction(true, "FB Login Success!");
//            SignInUser(token);
//        }
//        else {
//            SignInAction(false, "FB: User cancelled login");
//        }
//    }

//    private void PostToDatabase(string userId, PlayerData data) {
//        RestClient.Put(PROJECT_URL_USERS + userId + ".json?", data).Then(success => {
//            PrintToConsole("Post with status code " + success.StatusCode);
//            RetrieveData(userId);
//        });
//    }


//    private void RetrieveData(string userId) {
//        RestClient.Get<PlayerData>(PROJECT_URL_USERS + userId + ".json?").Then(response => {
//            DataRetrieveSuccess(response);
//            PrintToConsole("Retrive success ");
//        }).Catch(error => {
//            PromptSetUsername();
//            PrintToConsole("Cannot retrieve data: " + error);
//        });
//    }

//    private void DataRetrieveSuccess(PlayerData response) {
//        if (String.IsNullOrWhiteSpace(response.username)) {
//            PromptSetUsername();
//        } else {
//            PrintToConsole("Logged in success: username: " + response.username);
//        }
//    }

//    private void PromptSetUsername() {
//        panelEnterName.gameObject.SetActive(true);
//    }

//    public void SetUsername(InputField inputField) {
//        if (!String.IsNullOrWhiteSpace(inputField.text)) {
//            PlayerData data = SaveSystem.LoadPlayer();
//            data.username = inputField.text;
//            PostToDatabase(userId, data);
//            panelEnterName.gameObject.SetActive(false);
//        }
//    }

//    private void SignInUser(AccessToken accessToken) {
//        string userData = "{\"postBody\":\"access_token=" + accessToken.TokenString + "&providerId=facebook.com\",\"requestUri\":\"" + PROJECT_URL + "\",\"returnIdpCredential\":true,\"returnSecureToken\":true}";

//            RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key=" + WEB_API_KEY, userData)
//            .Then(response => {

//                userId = response.localId;
//                idToken = response.idToken;
//                refreshToken = response.refreshToken;

//                Debug.Log("Refresh token: " + response.refreshToken);
//                Debug.Log("Id token: " + response.idToken);
//                SignInSuccess(response.localId);

//            }).Catch(error => {
//                PrintToConsole("Sign In User failed: " + error.Message);
//            });

//        Debug.Log("Sending request: " + userData);
//    }

//    private void SignInSuccess(string localId) {
//        RetrieveData(localId);
//    }

//    private void RefreshToken() {
//        string refreshString = "{\"grant_type\":\"refresh_token\", \"refresh_token\":\""+ refreshToken +"\"}";
//        RestClient.Post<RefreshResponse>("https://securetoken.googleapis.com/v1/token?key=" + WEB_API_KEY, refreshString)
//        .Then(response => {
//            RefreshStatus(response);
//        }).Catch(e => {
//            PrintToConsole(e.Message);
//        });
//    }

//    private void RefreshStatus(RefreshResponse response) {
//        refreshToken = response.refresh_token;
//    }
//}

////[Serializable]
////public class SignResponse
////{
////    public string localId;
////    public string idToken;
////    public string refreshToken;
////}

////[Serializable]
////public class RefreshResponse
////{
////    public string refresh_token;
////}
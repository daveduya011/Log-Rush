using Facebook.Unity;
using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseConfig
{
    public const string WEB_API_KEY = "AIzaSyA8gXjFTBIN4gykvCcZPbsUKqZTiwx7t-Q";
    public const string PROJECT_ID = "log-rush";
    public const string PROJECT_URL = "https://" + PROJECT_ID + ".firebaseio.com";
    public const string PROJECT_URL_USERS = PROJECT_URL + "/users";

    public static RSG.IPromise<SignResponse> SignInToken(string accessToken) {
        string userString = "{\"postBody\":\"access_token=" + accessToken + "&providerId=facebook.com\",\"requestUri\":\"" + PROJECT_URL + "\",\"returnIdpCredential\":true,\"returnSecureToken\":true}";
        RSG.IPromise<SignResponse> response = RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key=" + WEB_API_KEY, userString);
        response.Then(data => {
            Debug.Log("Sign in success");
        }).Catch(e => {
            Debug.Log("Error on SigInToken(): + " + e.Message);
        });

        return response;
    }

    public static RSG.IPromise<RefreshResponse> RefreshToken(string refreshToken) {
        string refreshString = "{\"grant_type\":\"refresh_token\", \"refresh_token\":\"" + refreshToken + "\"}";

        RSG.IPromise<RefreshResponse> response = RestClient.Post<RefreshResponse>("https://securetoken.googleapis.com/v1/token?key=" + WEB_API_KEY, refreshString);

        response.Catch(e => {
            Debug.Log("Error on RefreshToken(): + " + e.Message);
        });

        return response;
    }

    public static RSG.IPromise<ResponseHelper> SaveUserData(string userId, string idToken, PlayerData data) {

        SaveSystem.SavePlayer(data);
        RSG.IPromise<ResponseHelper> response = RestClient.Put(PROJECT_URL_USERS + "/" + userId + ".json?auth=" + idToken, data);
        response.Then(a => {
            Debug.Log("Saved user data");
        });
        response.Catch(e => {
            Debug.Log("Error on SaveUserData(): + " + e.Message);
        });
        return response;
    }
    public static RSG.IPromise<ResponseHelper> SetLastUserLogin(string userId, string idToken) {

        var response = RestClient.Request(new RequestHelper {
            Uri = PROJECT_URL_USERS + "/" + userId + ".json",
            Params = new Dictionary<string, string> {
                { "auth", idToken }
            },
            Method = "PATCH",
            BodyString = "{\"lastLoginTime\":\""+ DateTime.Now.TotalSeconds() +"\"}",
        });
        response.Catch(e => {
            Debug.Log("Error on PostNewUser(): + " + e.Message);
        });
        return response;
    }

    public static RSG.IPromise<PlayerData> LoadUserData(string userId, string idToken) {
        RSG.IPromise<PlayerData> response = RestClient.Get<PlayerData>(PROJECT_URL_USERS + "/" + userId + ".json?auth=" + idToken);
        response.Then(data => {
            Debug.Log("Retrieved on LoadUserData(): " + data.username);
        }).Catch(error => {
            Debug.Log("Error on LoadUserData():  " + error);
        });

        return response;
    }


    public async static Task<Dictionary<string, PlayerData>> GetUsers() {
        fsSerializer serializer = new fsSerializer();

        bool hasResult = false;
        RSG.IPromise<ResponseHelper> response = RestClient.Get(PROJECT_URL_USERS + ".json");
        Dictionary<string, PlayerData> playerDatas = null;
        response.Then(data => {
            var json = fsJsonParser.Parse(data.Text);
            Dictionary<string, PlayerData> users = null;
            serializer.TryDeserialize(json, ref users);
            playerDatas = users;
            hasResult = true;
        });

        response.Catch(error => {
            Debug.Log("Error on GetUsers():  " + error);
            hasResult = true;
        });

        while (!hasResult) {
            await Task.Yield();
        }

        return playerDatas;
    }

}

[Serializable]
public class SignResponse
{
    public string localId;
    public string idToken;
    public string refreshToken;
    public string expiresIn;
}

[Serializable]
public class RefreshResponse
{
    public string refresh_token;
    public string expires_in;
}
[Serializable]
public class NewUserRegister
{
    public string lastLoginTime;
}
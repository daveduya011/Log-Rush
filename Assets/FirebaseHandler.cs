using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseHandler : MonoBehaviour
{
    //public static event Action<bool, string> SignInAction = delegate { };
    //public static event Action<object, EventArgs> AuthStateChanged = delegate { };
    //[HideInInspector]
    //public bool isInitialized;
    //[HideInInspector]
    //public FirebaseApp app;
    //[HideInInspector]
    //public FirebaseAuth auth;


    //private static FirebaseHandler _instance;

    //public static FirebaseHandler Instance { get {
    //        if (_instance != null ) return _instance;
    //        var go = new GameObject { name = nameof(FirebaseHandler) };
    //        DontDestroyOnLoad(go);
    //        _instance = go.AddComponent<FirebaseHandler>();
    //        return Instance;
    //    }
    //}

    //public async void Initialize() {
    //    var result = await FirebaseApp.CheckAndFixDependenciesAsync();

    //    if (result == DependencyStatus.Available) {
    //        app = FirebaseApp.DefaultInstance;
    //        auth = FirebaseAuth.DefaultInstance;
    //        auth.StateChanged += AuthOnStateChanged;

    //        if (!FB.IsInitialized) {
    //            // Initialize the Facebook SDK
    //            FB.Init(FBInitCallback, OnHideUnity);
    //        }
    //        else {
    //            // Already initialized, signal an app activation App Event
    //            FB.ActivateApp();
    //        }
    //    } else {
    //        SignInAction(false, "Could not resolve all firebase dependencies");
    //    }
    //}

    //private void FBInitCallback() {
    //    if (FB.IsInitialized) {
    //        // Signal an app activation App Event
    //        FB.ActivateApp();
    //        isInitialized = true;
    //        SignInAction(true, "Facebook SDK successfully initialized");

    //        // Continue with Facebook SDK
    //        // ...
    //    }
    //    else {
    //        SignInAction(false, "Failed to Initialize the Facebook SDK");
    //    }
    //}

    //private void OnHideUnity(bool isGameShown) {
    //    if (!isGameShown) {
    //        // Pause the game - we will need to hide
    //        Time.timeScale = 0;
    //    }
    //    else {
    //        // Resume the game - we're getting focus again
    //        Time.timeScale = 1;
    //    }
    //}

    //public void LoginWithFacebook() {
    //    var perms = new List<string>() { "id", "user_friends" };
    //    if (!FB.IsLoggedIn) {
    //        FB.LogInWithReadPermissions(perms, OnFacebookLoginResult);
    //    }
    //}
    //private void OnFacebookLoginResult(ILoginResult result) {
    //    if (FB.IsLoggedIn) {
    //        var aToken = AccessToken.CurrentAccessToken;
    //        SignInAction(true, "FB Login Success!");
    //        SignIntoFirebase(aToken);
    //    }
    //    else {
    //        SignInAction(false, "FB: User cancelled login");
    //    }
    //}

    //private void SignIntoFirebase(AccessToken accessToken) {
    //    var credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);
    //    auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
    //        if (task.IsCanceled) {
    //            SignInAction(false, "Firebase Login Cancelled!");
    //            return;
    //        }
    //        if (task.IsFaulted) {
    //            SignInAction(false, "Firebase Login Faulted: " + task.Exception);
    //            return;
    //        }

    //        FirebaseUser newUser = task.Result;
    //        SignInAction(true, "User signed in successfully with Name: " +
    //            newUser.DisplayName + ", id: " + newUser.UserId);
    //    });
    //}



    //private void AuthOnStateChanged(object sender, EventArgs e) {
    //    AuthStateChanged(sender, e);
    //}
}

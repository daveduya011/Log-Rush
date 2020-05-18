using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAdsScript : MonoBehaviour, IUnityAdsListener
{
    string gameId = "3556192";
    string myPlacementId = "rewardedVideo";
    bool testMode = false;

    private bool isFinished;
    private bool isSuccess;
    // Initialize the Ads listener and service:
    public async Task<bool> Show() {

        if (Advertisement.IsReady(myPlacementId)) {
            Advertisement.Show(myPlacementId);
            Advertisement.AddListener(this);
        }
        else {
            Advertisement.AddListener(this);
            Advertisement.Initialize(gameId, testMode);
        }
        

        while (!isFinished) {
            await Task.Yield();
        }
        gameObject.SetActive(false);
        Advertisement.RemoveListener(this);

        if (isSuccess) {
            return true;
        }
        return false;
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
            isSuccess = true;
        }
        else if (showResult == ShowResult.Skipped) {
            isSuccess = false;
        }
        else if (showResult == ShowResult.Failed) {
            isSuccess = false;
        }
        isFinished = true;
    }

    public void OnUnityAdsReady(string placementId) {
        Debug.Log("ads ready");
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId) {
            Advertisement.Show(myPlacementId);
            Debug.Log("ads shown");
        }
    }

    public void OnUnityAdsDidError(string message) {
        // Log the error.
        isSuccess = false;
        isFinished = true;
        Debug.Log("ads error");
    }

    public void OnUnityAdsDidStart(string placementId) {
        // Optional actions to take when the end-users triggers an ad.
        Debug.Log("ads started");
    }
}

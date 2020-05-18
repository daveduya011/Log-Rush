using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdReviveCanvas : MonoBehaviour
{
    public int maxNumOfWatchedAds = 1;
    public int revivePrice = 500;
    public float maxTimeInSeconds = 10;

    private int price;
    private int numOfWatchedAds;
    private int numOfRevivesByPrice;
    private float currentTime;
    private float tempCurrentTime;

    public LogManager logManager;
    public RectTransform watchAdsButton;
    public Image timerSlider;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI priceText;
    public Button payButton;
    public Button watchButton;
    public CoinUpdater coinUpdater;
    public RewardedAdsScript rewardedAds;

    private bool hasEnded;

    // Start is called before the first frame update
    void OnEnable()
    {
        hasEnded = false;
        currentTime = maxTimeInSeconds;

        payButton.interactable = true;
        watchButton.interactable = true;

        // When started, check if numOfWatched ads is less than max
        if (numOfWatchedAds >= maxNumOfWatchedAds) {
            watchAdsButton.gameObject.SetActive(false);
        }

        // set the price and double it every revive by price
        price = revivePrice * (numOfRevivesByPrice + 1);
        priceText.text = price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEnded)
            return;

        if (currentTime > 0) {
            currentTime -= Time.deltaTime;
            // update seconds text every seconds
            if ((int)currentTime != (int)tempCurrentTime) {
                timerText.text = ((int)currentTime).ToString();
            }
            tempCurrentTime = currentTime;
        } else {
            GameOver();
            hasEnded = true;
        }

        timerSlider.fillAmount = 1 / (maxTimeInSeconds / currentTime);
    }

    public async void WatchAd() {
        //gameObject.SetActive(false);
        hasEnded = true;
        payButton.interactable = false;
        watchButton.interactable = false;
        string initText = watchButton.GetComponentInChildren<TextMeshProUGUI>().text;
        watchButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Loading Ad");
        RewardedAdsScript ads = Instantiate(rewardedAds);
        bool isSuccess = await ads.Show();

        payButton.interactable = true;
        watchButton.interactable = true;
        watchButton.GetComponentInChildren<TextMeshProUGUI>().SetText(initText);

        if (isSuccess) {
            numOfWatchedAds++;
            logManager.Revive();
            gameObject.SetActive(false);
        }
        else {
            await PromptCanvas.Instance.Show("failed to play ads", "sorry but you failed to play the ads. try having an internet connection.", "Okay");
            hasEnded = false;
        }
    }

    public async void BuyRevive() {
        PlayerData playerData = SaveSystem.LoadPlayer();

        if (playerData.coins < price) {
            hasEnded = true;

            await PromptCanvas.Instance.Show("Insufficient Coins", "You don't have enough coins to revive.", "Okay");
            hasEnded = false;
            return;
        }

        // deduct from coins
        coinUpdater.AddCoins(-price);
        gameObject.SetActive(false);
        logManager.Revive();
        numOfRevivesByPrice++;
    }

    public void GameOver() {
        BGSoundSystem.Instance.PlayBGGameOver();
        GameManager.Instance.GameOver();
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}

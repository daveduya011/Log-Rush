using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUpdater : MonoBehaviour
{
    private TextMeshProUGUI text;
    private int numOfCoins;
    private int clickTimes;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        numOfCoins = SaveSystem.LoadPlayer().coins;
        text.text = numOfCoins.ToString();
    }

    public void AddCoins(int value) {
        PlayerData data = SaveSystem.LoadPlayer();
        numOfCoins += value;
        data.coins = numOfCoins;
        SaveSystem.SavePlayer(data);
        text.text = numOfCoins.ToString();
    }

    public void UpdateCoins(int value) {
        text.text = value.ToString();
    }

    public void Click() {
        clickTimes++;
        if (clickTimes % 10 == 0) {
            AddCoins(500);
        }
    }
}

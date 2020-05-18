using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    public DefaultPlayerSprite profileImage;
    public WaterDefaultColor profileColor;
    public Text textUsername;
    public Text textLevel;
    public Slider levelSlider;
    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerInfo(SaveSystem.LoadPlayer());
    }

    private void LoadPlayerInfo(PlayerData data) {
        PlayerData player = GameManager.Instance.GetPlayerData();

        if (!string.IsNullOrEmpty(player.username)) {
            textUsername.text = player.username;
        }
        levelSlider.value = player.levelProgress;
        textLevel.text = "Level " + player.level.ToString();
    }

    public void ReloadAll() {
        PlayerData data = SaveSystem.LoadPlayer();
        LoadPlayerInfo(data);
        profileImage.LoadDefaultSprite(data);
        profileColor.LoadDefaultColor(data);
    }
}

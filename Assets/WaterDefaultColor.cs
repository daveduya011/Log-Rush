using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterDefaultColor : MonoBehaviour
{
    public SkinLibrary skinLibrary;
    public SpriteRenderer sprite;
    public Image image;
    public enum WaterColor { RIVERCOLOR, SPLASHCOLOR }
    public WaterColor waterColor = WaterColor.RIVERCOLOR;

    [HideInInspector]
    public Color defaultColor;

    void Awake() {
        PlayerData data = GameManager.Instance.GetPlayerData();
        LoadDefaultColor(data);
    }
    public void LoadDefaultColor(PlayerData data) {

        if (data.equippedRiverId == null)
            return;

        if (sprite == null && image == null)
            return;

        defaultColor = sprite != null ? sprite.color : image != null ? image.color : Color.white;
        foreach (SkinAsset asset in skinLibrary.riverAssets) {
            Color color = waterColor == WaterColor.RIVERCOLOR ? ((RiverAsset)asset).riverColor : ((RiverAsset)asset).splashColor;
            if (data.equippedRiverId.Equals(asset.id)) {
                SetColor(color);
                return;
            }

            if (asset.isEquipped) {
                defaultColor = color;
            }
        }

        SetColor(defaultColor);
    }

    public void LoadDefaultColor(string equippedRiverId) {
        PlayerData playerData = new PlayerData();
        playerData.equippedRiverId = equippedRiverId;
        LoadDefaultColor(playerData);
    }

    private void SetColor(Color color) {
        if (sprite != null)
            sprite.color = color;
        else if (image != null)
            image.color = color;
    }
}

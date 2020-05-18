using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDefaultColor : MonoBehaviour
{
    public SkinLibrary skinLibrary;
    public Text text;

    void Start() {
        LoadDefaultColor();
    }
    private void LoadDefaultColor() {
        PlayerData data = GameManager.Instance.GetPlayerData();

        if (data.equippedRiverId == null)
            return;

        Color defaultColor = text.color;
        foreach (SkinAsset asset in skinLibrary.logAssets) {
            if (data.equippedLogId.Equals(asset.id)) {
                text.color = ((LogAsset)asset).textColor;
                return;
            }

            if (asset.isEquipped) {
                defaultColor = ((LogAsset)asset).textColor;
            }
        }

        text.color = defaultColor;
    }
}

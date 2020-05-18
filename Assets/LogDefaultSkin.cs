using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDefaultSkin : MonoBehaviour
{
    public SpriteRenderer logSprite;
    public SkinLibrary skinLibrary;

    // Start is called before the first frame update
    void Start()
    {
        LoadDefaultLog();
    }

    private void LoadDefaultLog() {
        PlayerData data = GameManager.Instance.GetPlayerData();

        if (data.equippedLogId == null)
            return;

        Sprite defaultSprite = logSprite.sprite;
        foreach (SkinAsset asset in skinLibrary.logAssets) {
            if (data.equippedLogId.Equals(asset.id)) {
                logSprite.sprite = asset.image;
                return;
            }

            if (asset.isEquipped) {
                defaultSprite = asset.image;
            }
        }

        logSprite.sprite = defaultSprite;
    }

}

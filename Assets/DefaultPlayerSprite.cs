using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultPlayerSprite : MonoBehaviour
{
    public SkinLibrary skinLibrary;
    public SpriteRenderer sprite;
    public Image image;
    public enum CharacterView { PROFILE, GAMEPLAY }
    public CharacterView characterView = CharacterView.PROFILE;

    void Awake() {
        PlayerData data = GameManager.Instance.GetPlayerData();
        LoadDefaultSprite(data);
    }
    public void LoadDefaultSprite(PlayerData data) {
        if (data.equippedCharacterId == null)
            return;
        if (sprite == null && image == null)
            return;

        Sprite defaultSprite = sprite != null ? sprite.sprite : image != null ? image.sprite : null;

        foreach (SkinAsset asset in skinLibrary.characterAssets) {
            Sprite sprite = characterView == CharacterView.PROFILE ? ((CharacterAsset)asset).displayImage : ((CharacterAsset)asset).image;

            if (data.equippedCharacterId.Equals(asset.id)) {
                SetSprite(sprite);
                return;
            }

            if (asset.isEquipped) {
                defaultSprite = sprite;
            }
        }

        SetSprite(defaultSprite);
    }

    public void LoadDefaultSprite(string equippedCharacterId) {
        PlayerData playerData = new PlayerData();
        playerData.equippedCharacterId = equippedCharacterId;
        LoadDefaultSprite(playerData);
    }

    private void SetSprite(Sprite spriteImage) {
        if (sprite != null)
            sprite.sprite = spriteImage;
        else if (image != null)
            image.sprite = spriteImage;
    }
}

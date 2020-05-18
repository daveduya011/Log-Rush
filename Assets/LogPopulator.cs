using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class LogPopulator : SkinsPopulator
{

    public override void FillData(ItemData data, SkinAsset asset, bool isOwned, bool isEquipped) {
        transform.Find("image").GetComponent<Image>().sprite = asset.image;
        transform.Find("back").GetComponent<Image>().sprite = ((LogAsset)asset).crocodileImageBack;
        transform.Find("front").GetComponent<Image>().sprite = ((LogAsset)asset).crocodileImageFront;
        transform.Find("name").GetComponent<Text>().text = asset.itemName;
    }

    public override List<SkinAsset> SetAssets(SkinLibrary library) {
        return library.logAssets.ConvertAll(x => (SkinAsset)x);
    }

    public override string GetEquippedId(PlayerData data) {
        return data.equippedLogId;
    }

    public override List<string> GetOwnedId(PlayerData data) {
        return data.ownedLogsId;
    }

    public override void AddToOwnedList(PlayerData data, string id) {

        if (data.ownedLogsId == null) {
            data.ownedLogsId = new List<string>();
        }
        data.ownedLogsId.Add(id);
    }

    public override void SetEquippedId(PlayerData data, string id) {
        data.equippedLogId = id;
    }
}


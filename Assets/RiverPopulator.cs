using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class RiverPopulator : SkinsPopulator
{
    public override void FillData(ItemData data, SkinAsset asset, bool isOwned, bool isEquipped) {
        transform.Find("riverColor").GetComponent<Image>().color = ((RiverAsset)asset).riverColor;
        transform.Find("splashColor").GetComponent<Image>().color = ((RiverAsset)asset).splashColor;
        transform.Find("name").GetComponent<Text>().text = asset.itemName;

        data.SetColor(((RiverAsset)asset).riverColor);
    }

    public override List<SkinAsset> SetAssets(SkinLibrary library) {
        return library.riverAssets.ConvertAll(x => (SkinAsset)x);
    }

    public override string GetEquippedId(PlayerData data) {
        return data.equippedRiverId;
    }

    public override List<string> GetOwnedId(PlayerData data) {
        return data.ownedRiversId;
    }

    public override void AddToOwnedList(PlayerData data, string id) {

        if (data.ownedRiversId == null) {
            data.ownedRiversId = new List<string>();
        }
        data.ownedRiversId.Add(id);
    }

    public override void SetEquippedId(PlayerData data, string id) {
        data.equippedRiverId = id;
    }
}

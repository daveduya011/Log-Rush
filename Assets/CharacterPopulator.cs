using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class CharacterPopulator : SkinsPopulator
{
    public override void FillData(ItemData data, SkinAsset asset, bool isOwned, bool isEquipped) {
        transform.Find("image").GetComponent<Image>().sprite = asset.image;
        transform.Find("displayImage").GetComponent<Image>().sprite = ((CharacterAsset)asset).displayImage;
        transform.Find("name").GetComponent<Text>().text = asset.itemName;
        transform.Find("description").GetComponent<Text>().text = asset.description;
    }

    public override List<SkinAsset> SetAssets(SkinLibrary library) {
        return library.characterAssets.ConvertAll(x => (SkinAsset)x);
    }

    public override string GetEquippedId(PlayerData data) {
        return data.equippedCharacterId;
    }

    public override List<string> GetOwnedId(PlayerData data) {
        return data.ownedCharactersId;
    }


    public override void AddToOwnedList(PlayerData data, string id) {

        if (data.ownedCharactersId == null) {
            data.ownedCharactersId = new List<string>();
        }
        data.ownedCharactersId.Add(id);
    }

    public override void SetEquippedId(PlayerData data, string id) {
        data.equippedCharacterId = id;
    }
}

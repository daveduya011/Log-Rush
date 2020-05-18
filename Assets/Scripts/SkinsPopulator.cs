using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkinsPopulator : MonoBehaviour
{
    protected ItemData activeData;
    protected List<ItemData> itemDatas;

    public ScriptableObject library;
    public RectTransform panelToPopulate;
    public ItemData prefab;

    public CoinUpdater coinUpdater;

    void Start() {
        itemDatas = new List<ItemData>();
        Load();
    }

    public virtual void Load() {

        SkinLibrary library = this.library as SkinLibrary;
        List<SkinAsset> assets = SetAssets(library);
        if (assets.Count == 0)
            return;

        int i = 0;
        transform.Find("button").GetComponent<Button>().onClick.AddListener(delegate { BuyItem(); });

        foreach (SkinAsset asset in assets) {
            ItemData data = Instantiate(prefab, panelToPopulate.transform);
            FillData(data, asset);


            if (i == 0) {
                activeData = data;
                data.toggle.isOn = true;
                i++;
            }

            data.toggle.onValueChanged.AddListener((isSelected) => {
                if (isSelected) {
                    FillData(data, asset);
                    activeData = data;
                }
            });


            itemDatas.Add(data);
        }

        FillData(activeData, activeData.data);
    }

    public abstract List<SkinAsset> SetAssets(SkinLibrary library);

    public virtual void FillData(ItemData data, SkinAsset asset) {
        data.Register(asset);
        data.toggle.group = GetComponent<ToggleGroup>();

        bool isOwned = CheckIfOwned(asset);
        bool isEquipped = CheckIfEquipped(asset);

        data.SetImage(asset.image);
        data.SetOwned(isOwned);

        if (isOwned) {
            // add the owned ribbon

            if (!isEquipped) {
                transform.Find("button").GetComponentInChildren<TextMeshProUGUI>().text = "use";
                transform.Find("button").GetComponent<Button>().interactable = true;
            }
            else {
                foreach (ItemData tempData in itemDatas)
                    tempData.SetEquipped(false);
                data.SetEquipped(true);
                transform.Find("button").GetComponentInChildren<TextMeshProUGUI>().text = "active";
                transform.Find("button").GetComponent<Button>().interactable = false;
            }
        }
        else {
            transform.Find("button").GetComponentInChildren<TextMeshProUGUI>().text = data.data.price.ToString();
            transform.Find("button").GetComponent<Button>().interactable = true;

            if (!asset.isActive) {
                transform.Find("button").GetComponentInChildren<TextMeshProUGUI>().text = "item closed";
                transform.Find("button").GetComponent<Button>().interactable = false;
            }
        }

        FillData(data, asset, isOwned, isEquipped);
    }

    private bool CheckIfEquipped(SkinAsset asset) {
        string id = GetEquippedId();
        if (id == "") {
            if (asset.isEquipped)
                return true;
        }
        return asset.id.Equals(id);
    }

    private bool CheckIfOwned(SkinAsset asset) {
        if (asset.isOwned)
            return true;
        List<string> idList = GetOwnedId();

        // Check for owned items
        if (idList != null) {
            if (idList.Contains(asset.id)) {
                return true;
            }
        }
        return false;
    }

    private string GetEquippedId() {
        PlayerData data = SaveSystem.LoadPlayer();
        return GetEquippedId(data);
    }
    private List<string> GetOwnedId() {
        PlayerData data = SaveSystem.LoadPlayer();
        return GetOwnedId(data);
    }

    public abstract string GetEquippedId(PlayerData data);
    public abstract List<string> GetOwnedId(PlayerData data);



    public void BuyItem() {
        PlayerData data = SaveSystem.LoadPlayer();
        SkinAsset item = activeData.data;

        bool isOwned = CheckIfOwned(item);
        bool isEquipped = CheckIfEquipped(item);

        // If owned, then equip
        if (isOwned) {
            if (!isEquipped) {
                SetEquippedId(data, item.id);
            }
        }
        else {
            if (data.coins < item.price) {
                PromptCanvas.Instance.Show("Insufficient Coins", "You don't have enough coins to buy the item.", "Okay");
                return;
            }

            // if not owned, then add to database
            AddToOwnedList(data, item.id);
            SetEquippedId(data, item.id);

            // deduct
            data.coins -= item.price;
            //update coins text
            coinUpdater.UpdateCoins(data.coins);
        }


        SaveSystem.SavePlayer(data);
        FillData(activeData, item);
    }

    public abstract void SetEquippedId(PlayerData data, string id);

    public abstract void AddToOwnedList(PlayerData data, string id);
    public abstract void FillData(ItemData data, SkinAsset asset, bool isOwned, bool isEquipped);
}

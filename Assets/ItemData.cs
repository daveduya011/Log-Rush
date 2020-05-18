using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ItemData : MonoBehaviour
{
    public SkinAsset data;
    public Toggle toggle;

    public GameObject isEquipped;
    public GameObject isOwned;
    public Image image;

    public void Register(SkinAsset data) { 
        this.data = data;
        isOwned.SetActive(false);
        isEquipped.SetActive(false);
    }

    public void SetOwned(bool isOn) {
        this.isOwned.SetActive(isOn);
    }
    public void SetEquipped(bool isOn) {
        isEquipped.SetActive(isOn);
    }
    public void SetImage(Sprite image) {
        this.image.sprite = image;
    }
    public void SetColor(Color color) {
        this.image.color = color;
    }
}

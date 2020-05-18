using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomProfile : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    public WaterDefaultColor waterDefaultColor;
    public DefaultPlayerSprite defaultPlayerSprite;
    public Color readyColor;
    public Color defaultColor;

    public bool isReady;
    private string equippedCharacterId;
    private string equippedRiverId;

    private GameObject parentObject;

    private PhotonView PV;
    void Awake() {
        PV = GetComponent<PhotonView>();
        defaultColor = GetComponent<Image>().color;
    }

    public void SetReady(bool cond) {
        if (PV.IsMine) {
            isReady = cond;
            GetComponent<Image>().color = isReady ? readyColor : defaultColor;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(isReady);
            stream.SendNext(equippedCharacterId);
            stream.SendNext(equippedRiverId);
        }
        else {
            // Network player, receive data
            this.isReady = (bool)stream.ReceiveNext();
            this.equippedCharacterId = (string)stream.ReceiveNext();
            this.equippedRiverId = (string)stream.ReceiveNext();
            UpdateProfile();
        }
    }

    private void UpdateProfile() {
        if (equippedCharacterId != null)
            defaultPlayerSprite.LoadDefaultSprite(equippedCharacterId);
        if (equippedRiverId != null)
            waterDefaultColor.LoadDefaultColor(equippedRiverId);

        if (isReady) {
            GetComponent<Image>().color = readyColor;
        } else {
            GetComponent<Image>().color = defaultColor;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {

        info.Sender.TagObject = this.gameObject;
        parentObject = GameObject.FindGameObjectWithTag("PlayerProfilesPanel");
        transform.SetParent(parentObject.transform);
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentObject.GetComponent<RectTransform>());

        if (PV.IsMine) {
            PlayerData data = SaveSystem.LoadPlayer();
            equippedCharacterId = data.equippedCharacterId;
            equippedRiverId = data.equippedRiverId;
        }

        UpdateProfile();

    }
}

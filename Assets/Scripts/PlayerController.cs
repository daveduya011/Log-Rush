﻿using System;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    private string equippedCharacterId;

    public float drownTime = 2f;
    private LogManager logManager;
    private CameraController cameraController;
    private Animator animator;
    private float tempDrownTime;
    public SpriteRenderer characterSprite;
    public SkinLibrary skinLibrary;

    private PhotonView PV;
    public Color otherPlayerColor;

    public bool isFinishLineReached;

    public enum State
    {
        Jumping,
        Idle,
        Drowning,
        Dead
    }
    public State state = State.Idle;
    public Vector3 targetPos;
    private float jumpSpeed;

    void Awake() {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<CameraController>();
        jumpSpeed = cameraController.focusSpeed;
        logManager = GameObject.FindGameObjectWithTag("LogManager").GetComponent<LogManager>();

        PlayerData data = GameManager.Instance.GetPlayerData();
        LoadDefaultPlayer(data);

        if (PV != null && PV.IsMine) {
            gameObject.tag = "Player";
        }
    }

    void Start() {
        
    }

    private void LoadDefaultPlayer(PlayerData data) {
        if (data.equippedCharacterId == null)
            return;

        Sprite defaultSprite = characterSprite.sprite;
        foreach (SkinAsset asset in skinLibrary.characterAssets) {
            if (data.equippedCharacterId.Equals(asset.id)) {
                characterSprite.sprite = asset.image;
                return;
            }

            if (asset.isEquipped) {
                defaultSprite = asset.image;
            }
        }
        
        characterSprite.sprite = defaultSprite;
    }

    void Update() {
        if (isFinishLineReached)
            return;

        if (PV != null && !PV.IsMine) {
            return;
        }

        if (state == State.Jumping) {
            Jump();
        }
        
        if (state == State.Dead) {
            Jump();
            return;
        }

        if (state == State.Idle) {
            if (transform.position.y < cameraController.transform.position.y - Camera.main.orthographicSize) {
                state = State.Drowning;
                tempDrownTime = drownTime;
                if (!logManager.isSpawnCrocodile) {
                    logManager.drowningPanel.gameObject.SetActive(true);
                    cameraController.GetComponent<CustomImageEffect>().ActivateEffect(true);
                    FXSoundSystem.Instance.PlaySound(logManager.drowningSound);
                }
                
            }
        }
        

        if (state == State.Drowning) {
            if (tempDrownTime > 0) {
                tempDrownTime -= Time.deltaTime;
            } else {
                state = State.Dead;
                logManager.GameOver();
            }
        }
        
    }

    private void Jump() {
        Vector3 pos = transform.position;

        pos.y = targetPos.y;
        transform.position =
        Vector3.Lerp(transform.position, pos, jumpSpeed);

        if (transform.position.y >= pos.y - 1f) {
            state = State.Idle;
            animator.SetBool("isJumping", false);
        }
    }

    public void JumpToPosition(Vector3 pos) {
        if (state == State.Drowning) {
            logManager.drowningPanel.gameObject.SetActive(false);
            cameraController.GetComponent<CustomImageEffect>().ActivateEffect(false);
            FXSoundSystem.Instance.Stop();
        }
        targetPos = pos;
        state = State.Jumping;
        animator.SetBool("isJumping", true);
    }

    public void Drown(Vector3 pos) {
        JumpToPosition(pos);
        animator.SetBool("isDrowned", true);
        FXSoundSystem.Instance.PlaySound(logManager.jumpInWaterSound, 0.5f);

        if (PV != null && PV.IsMine) {
            PV.RPC("RPC_SendDrowned", RpcTarget.Others);
            PV.RPC("RPC_SendFinishLineReached", RpcTarget.All, false);
        }
    }

    public async void CollectCoins() {
        animator.SetBool("isCoinCollected", true);
        FXSoundSystem.Instance.PlaySound(logManager.coinCollectSound);
        await Task.Delay(100);
        animator.SetBool("isCoinCollected", false);
    }

    public void Revive() {
        tempDrownTime = drownTime;
        state = State.Idle;
        logManager.drowningPanel.gameObject.SetActive(false);
        cameraController.GetComponent<CustomImageEffect>().ActivateEffect(false);
        animator.SetBool("isDrowned", false);
    }

    [PunRPC]
    public void RPC_SendDrowned() {
        FXSoundSystem.Instance.PlaySound(logManager.jumpInWaterSound, 0.5f);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        info.Sender.TagObject = this.gameObject;

        if (!PV.IsMine) {
            characterSprite.color = otherPlayerColor;
            Vector3 pos = transform.position;
            pos.z += 1;
        }
        if (PV.IsMine) {
            equippedCharacterId = SaveSystem.LoadPlayer().equippedCharacterId;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(equippedCharacterId);
        }
        else {
            // Network player, receive data
            this.equippedCharacterId = (string)stream.ReceiveNext();
            UpdateData();
        }
    }

    private void UpdateData() {
        if (equippedCharacterId != null) {
            PlayerData data = GameManager.Instance.GetPlayerData();
            data.equippedCharacterId = equippedCharacterId;
            LoadDefaultPlayer(data);
        }
    }


    [PunRPC]
    public void RPC_SendFinishLineReached(bool hasReached) {
        // if has reached, it means it reached the finish line
        // if not, then it eventually got drowned

        logManager.isGameOver = true;
        isFinishLineReached = true;

        if (PV.IsMine) {
            GameManager.Instance.isWin = hasReached;
        } else {
            GameManager.Instance.isWin = !hasReached;
        }
        Hashtable hash = new Hashtable();
        hash.Add("Score", GameManager.Instance.score);

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        StartCoroutine(GameManager.Instance.GameOverMultiplayer());
    }

    public void SetFinishLineReached() {
        isFinishLineReached = true;
        PV.RPC("RPC_SendFinishLineReached", RpcTarget.All, true);
    }

}
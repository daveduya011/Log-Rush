using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string equippedCharacterId;

    public float drownTime = 2f;
    private LogManager logManager;
    private CameraController cameraController;
    private Animator animator;
    private float tempDrownTime;
    public SpriteRenderer characterSprite;
    public SkinLibrary skinLibrary;
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
        animator = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<CameraController>();
        jumpSpeed = cameraController.focusSpeed;
        logManager = GameObject.FindGameObjectWithTag("LogManager").GetComponent<LogManager>();

        PlayerData data = GameManager.Instance.GetPlayerData();
        LoadDefaultPlayer(data);
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

    private void UpdateData() {
        if (equippedCharacterId != null) {
            PlayerData data = GameManager.Instance.GetPlayerData();
            data.equippedCharacterId = equippedCharacterId;
            LoadDefaultPlayer(data);
        }
    }

    public void SetFinishLineReached() {
        isFinishLineReached = true;
    }
}

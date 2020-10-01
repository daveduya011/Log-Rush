using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum State
    {
        Focusing,
        Scrolling,
        Idle
    }
    public State state = State.Idle;
    public float focusSpeed = 0.1f;
    public Transform focusedObject;
    public float speed = 5;
    public float onCrocsSpawnedSpeed = 5;
    public float maxSpeed = 12.5f;

    public SkinLibrary skinLibrary;
    // Start is called before the first frame update
    void Start()
    {
        LoadDefaultRiver();
    }

    private void LoadDefaultRiver() {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data.equippedRiverId == null)
            return;

        Color defaultColor = Camera.main.backgroundColor;
        foreach (SkinAsset asset in skinLibrary.riverAssets) {
            if (data.equippedRiverId.Equals(asset.id)) {
                Camera.main.backgroundColor = ((RiverAsset)asset).riverColor;
                return;
            }

            if (asset.isEquipped) {
                defaultColor = ((RiverAsset)asset).riverColor;
            }
        }

        Camera.main.backgroundColor = defaultColor;
    }
    // Update is called once per frame
    void Update()
    {
        if (state == State.Idle)
            return;

        Vector3 pos = transform.position;
        pos.y += speed * Time.deltaTime;

        if (state == State.Focusing) {
            pos.y = focusedObject.position.y + 5f;
            transform.position =
            Vector3.Lerp(transform.position, pos, focusSpeed * Time.deltaTime);

            if (transform.position.y >= pos.y - 1f) {
                state = State.Scrolling;
            }
        } else {
            transform.position = pos;
        }

    }

    public void focusObject(Transform focusObject) {
        this.focusedObject = focusObject;
        state = State.Focusing;
    }

    public void focusObjectToCenter(Transform focusObject) {
        transform.position = new Vector3(transform.position.x, focusObject.position.y + 4f, transform.position.z);
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public float swipeThreshold = 50f;
    public bool detectSwipeOnlyAfterRelease = false;

    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;
    public UnityEvent OnSwipeDown;
    public UnityEvent OnSwipeUp;
    public UnityEvent OnTap;
    public UnityEvent OnAfterInputFinished;
    public UnityEvent BeforeInputFinished;
    public UnityEvent OnInputFinished;

    private Vector2 fingerDown;
    private Vector2 fingerUp;

    private TouchPhase touchPhase;

    public bool isSwiped;
    public bool isSwipedLeft;
    public bool isSwipedRight;
    public bool isSwipedDown;
    public bool isSwipedUp;
    public bool isTapped;

    private void Start() {
        touchPhase = detectSwipeOnlyAfterRelease ? TouchPhase.Ended : TouchPhase.Moved;

        OnSwipeLeft.AddListener(SwipeLeft);
        OnSwipeRight.AddListener(SwipeRight);
        OnSwipeDown.AddListener(SwipeDown);
        OnSwipeUp.AddListener(SwipeUp);
        OnTap.AddListener(Tap);
        OnInputFinished.AddListener(InputFinished);
    }

    private void AfterInputFinished() {
        isSwipedLeft = false;
        isSwipedRight = false;
        isSwipedDown = false;
        isSwipedUp = false;
        isTapped = false;

    }
    private void InputFinished() {
        BeforeInputFinished.Invoke();
        AfterInputFinished();
    }


    private void Update() {
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            this.fingerDown = Input.mousePosition;
            this.fingerUp = Input.mousePosition;
        }
        if (detectSwipeOnlyAfterRelease) {
            if (Input.GetMouseButtonUp(0)) {
                this.fingerDown = Input.mousePosition;
                this.CheckSwipe();
                this.CheckTap();
                isSwiped = false;
            }
        } else {
            if (Input.GetMouseButton(0)) {
                this.fingerDown = Input.mousePosition;
                this.CheckSwipe();
            }
            if (Input.GetMouseButtonUp(0)) {
                this.fingerDown = Input.mousePosition;
                this.CheckTap();
                isSwiped = false;
            }
        }
        #endif
        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                this.fingerDown = touch.position;
                this.fingerUp = touch.position;
            }

            if (touch.phase == touchPhase) {
                this.fingerDown = touch.position;
                this.CheckSwipe();
            }
            if (touch.phase == TouchPhase.Ended) {
                this.fingerDown = touch.position;
                this.CheckTap();
                isSwiped = false;
            }
        }
    }

    private void CheckTap() {
        float deltaX = this.fingerDown.x - this.fingerUp.x;
        float deltaY = fingerDown.y - fingerUp.y;

        if (Math.Abs(deltaX) < swipeThreshold && Math.Abs(deltaY) < swipeThreshold) {
            if (!isSwiped) {
                OnTap.Invoke();
                OnInputFinished.Invoke();
            }
        }
        this.fingerUp = this.fingerDown;
    }

    private void CheckSwipe() {

        if (VerticalMove() > swipeThreshold && VerticalMove() > HorizontalValMove()) {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp.Invoke();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown.Invoke();
            }
            OnInputFinished.Invoke();
            isSwiped = true;
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalValMove() > swipeThreshold && HorizontalValMove() > VerticalMove()) {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight.Invoke();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft.Invoke();
            }
            OnInputFinished.Invoke();
            isSwiped = true;
            fingerUp = fingerDown;
        }
    }
    float VerticalMove() {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float HorizontalValMove() {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }


    private void Tap() {
        isTapped = true;
    }

    private void SwipeUp() {
        isSwipedUp = true;
    }

    private void SwipeDown() {
        isSwipedDown = true;
    }

    private void SwipeRight() {
        isSwipedRight = true;

    }

    private void SwipeLeft() {
        isSwipedLeft = true;
    }

}
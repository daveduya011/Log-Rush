using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollController : MonoBehaviour, IPointerEnterHandler
{
    [HideInInspector]
    public bool isScrolling;
    private float targetValue;
    private ScrollRect scrollRect;

    void Start() {
        scrollRect = GetComponent<ScrollRect>();
    }
    public void ScrollLeft() {
        isScrolling = true;
        targetValue = scrollRect.horizontalScrollbar.value - 0.5f;
    } public void ScrollRight() {
        isScrolling = true;
        targetValue = scrollRect.horizontalScrollbar.value + 0.5f;
    }
    void Update() {
        if (!isScrolling)
            return;
        if (targetValue != scrollRect.horizontalScrollbar.value) {
            scrollRect.horizontalScrollbar.value = Mathf.Lerp(scrollRect.horizontalScrollbar.value, targetValue, 0.1f);
        } else {
            isScrolling = false;
        }
    }


    public void OnPointerEnter(PointerEventData eventData) {
        isScrolling = false;
    }
}

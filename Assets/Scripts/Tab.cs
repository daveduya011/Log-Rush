using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    public RectTransform panelToActivate;
    private Toggle toggle;
    private Animator animator;

    void Start() {
        toggle = GetComponent<Toggle>();
        OnToggle(toggle.isOn);

        toggle.onValueChanged.AddListener((isSelected) => {
            if (isSelected) {
                OnToggle(true);
            } else {
                OnToggle(false);
            }
        });
    }
    // Start is called before the first frame update
    public void OnToggle(bool isOn) {
        animator = GetComponent<Animator>();

        if (isOn) {
            animator.SetBool("isOn", true);

            

            if (panelToActivate == null)
                return;

            panelToActivate.gameObject.SetActive(true);
            Tab[] tabs = panelToActivate.GetComponentsInChildren<Tab>();
            foreach (Tab tab in tabs) {
                tab.OnToggle(tab.GetComponent<Toggle>().isOn);
            }
        }
        else {
            animator.SetBool("isOn", false);

            if (panelToActivate != null)
                panelToActivate.gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransition : MonoBehaviour
{
    Animator animator;

    public void TransitionOn()
    {
        gameObject.SetActive(true);
        animator = GetComponent<Animator>();
        animator.SetBool("isOn", true);
    }

    public void TransitionOff()
    {
        animator.SetBool("isOn", false);
    }
}

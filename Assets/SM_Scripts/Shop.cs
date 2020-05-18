using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Animator shopAnimation;

    public void btnShopOpen()
    {
        shopAnimation.gameObject.SetActive(true);
        shopAnimation.SetBool("isOpen", true);
        shopAnimation.SetBool("isClose", false);
    }

    public void btnShopClose()
    {
        shopAnimation.SetBool("isClose", true);
        shopAnimation.SetBool("isOpen", false);
    }
}

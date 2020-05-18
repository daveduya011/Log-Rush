using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PromptCanvas : MonoBehaviour
{
    public Text titleText;
    public Text contentText;
    public Text buttonText;

    public Animator animator;

    protected static PromptCanvas _instance;
    private bool isConfirmed;

    public static PromptCanvas Instance { get { return _instance; } }

    public void Awake() {
        if (_instance == null) {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    public async Task Show(string titleText, string contentText, string buttonText)
    {
        isConfirmed = false;
        gameObject.SetActive(true);

        this.titleText.text = titleText;
        this.contentText.text = contentText;
        this.buttonText.text = buttonText;

        animator.SetBool("isOn", true);

        while (!isConfirmed) 
            await Task.Yield();
    }

    public void Hide() {
        isConfirmed = true;
        animator.SetBool("isOn", false);
    }

}

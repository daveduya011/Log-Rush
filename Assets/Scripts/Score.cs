using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI text;
    public Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void PlayText(string text) {
        animator.Play("Score", 0, 0);
        this.text.text = text;
    }

    public void UpdateScore() {
        animator.Play("Score", 0, 0);
        text.text = GameManager.Instance.score.ToString();
    }
}

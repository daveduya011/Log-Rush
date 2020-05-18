using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Scoreboard scoreboard;
    public RectTransform panelEnterName;
    private InputField panelNameInput;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScore();
    }

    // Update is called once per frame
    public void UpdateScore() {
        scoreText.text = GameManager.Instance.score.ToString();
        scoreboard.UpdateScoreboard();

        if (GameManager.Instance.isNewHighScore) {
            panelEnterName.gameObject.SetActive(true);
        } else {
            panelEnterName.gameObject.SetActive(false);
        }
    }

    public void SetHighscoreName() {
        panelNameInput = panelEnterName.GetComponentInChildren<InputField>();

        PlayerData data = SaveSystem.LoadPlayer();
        data.highScoreNames[GameManager.Instance.scoreIndex] = panelNameInput.text;

        SaveSystem.SavePlayer(data);

        scoreboard.UpdateScoreboard();
        panelEnterName.gameObject.SetActive(false);
    }
}

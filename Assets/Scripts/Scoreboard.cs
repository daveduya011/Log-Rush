using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public RectTransform[] scorePanels;
    public Color highScoreColor;
    private Text[] score;
    private Text[] scoreboardName;

    void Start() {
        score = new Text[scorePanels.Length];
        scoreboardName = new Text[scorePanels.Length];

        for (int i = 0; i < scorePanels.Length; i++) {
            score[i] = scorePanels[i].Find("score").GetComponent<Text>();
            scoreboardName[i] = scorePanels[i].Find("name").GetComponent<Text>();
        }
    }
    public void UpdateScoreboard() {
        PlayerData data = SaveSystem.LoadPlayer();

        for (int i = 0; i < scorePanels.Length; i++) {
            score[i].text = data.highScores[i].ToString();
            scoreboardName[i].text = data.highScoreNames[i];
        }

        scorePanels[GameManager.Instance.scoreIndex].GetComponent<Image>().color = highScoreColor;
    }
    
}

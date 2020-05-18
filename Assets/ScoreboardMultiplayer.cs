using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardMultiplayer : MonoBehaviour
{
    public GameObject scoreRowPrefab;
    public RectTransform scoreContentPanel;
    // Start is called before the first frame update
    void Start()
    {
        int rank = 1;
        var list = GameManager.Instance.multiplayerScoreList;
        foreach(var item in list) {
            GameObject highscoreObject = Instantiate(scoreRowPrefab, scoreContentPanel.transform);
            highscoreObject.transform.Find("rank").GetComponent<Text>().text = rank.ToString();
            highscoreObject.transform.Find("name").GetComponent<Text>().text = item.Key;
            highscoreObject.transform.Find("score").GetComponent<Text>().text = item.Value.ToString();
            rank++;
        }
    }

}

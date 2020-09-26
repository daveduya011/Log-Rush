using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public bool isNewHighScore;
    public int currentLogIndex = 0;
    public int scoreMultiplier = 10;
    public int score = 0;
    public int coins;
    [HideInInspector]
    public int scoreIndex;
    [HideInInspector]
    public PlayerData playerData;
    [HideInInspector]
    public bool isWin;
    [HideInInspector]
    public List<KeyValuePair<string, int>> multiplayerScoreList;

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


    private void Awake() {
        if (_instance != null) {
            _instance.Reset();
            Destroy(this);
        }
        else {
            playerData = SaveSystem.LoadPlayer();
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }

    public void Reset() {
        isNewHighScore = false;
        currentLogIndex = 0;
        score = 0;
        coins = 0;
        scoreIndex = 0;
        isWin = false;
    }
    public int NextLog() {
        score += scoreMultiplier;
        currentLogIndex++;
        return currentLogIndex;
    }
    
    public void GameOver() {
        for (int i = 0; i < playerData.highScores.Length; i++) {
            if (score > playerData.highScores[i]) {
                for (int j = playerData.highScores.Length - 1; i < j; j--) {
                    playerData.highScores[j] = playerData.highScores[j - 1];
                    playerData.highScoreNames[j] = playerData.highScoreNames[j - 1];
                }

                playerData.highScores[i] = score;
                scoreIndex = i;
                isNewHighScore = true;
                
                break;
            }
        }
        
        PlayerData data = SaveSystem.LoadPlayer();
        data.highScoreNames = playerData.highScoreNames;
        data.highScores = playerData.highScores;
        data.coins += coins;

        SaveSystem.SavePlayer(data);
        QuitGame();
    }

    private void QuitGame() {
        SceneManager.LoadScene("GameOver");
    }

    public PlayerData GetPlayerData() {
        return playerData;
    }

    public void RefreshPlayerData(PlayerData playerData) {
        this.playerData = playerData;
    }

}

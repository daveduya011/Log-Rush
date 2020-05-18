using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public bool isNewHighScore;
    public int currentLogIndex = 0;
    public int scoreMultiplier = 10;
    public int score = 0;
    public int coins;

    [HideInInspector]
    public int[] highScores;
    public string[] highScoreNames;
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
            highScores = playerData.highScores;
            highScoreNames = playerData.highScoreNames;
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
        for (int i = 0; i < highScores.Length; i++) {
            if (score > highScores[i]) {
                for (int j = highScores.Length - 1; i < j; j--) {
                    highScores[j] = highScores[j - 1];
                    highScoreNames[j] = highScoreNames[j - 1];
                }

                highScores[i] = score;
                scoreIndex = i;
                isNewHighScore = true;
                
                break;
            }
        }
        
        PlayerData data = SaveSystem.LoadPlayer();
        data.highScoreNames = highScoreNames;
        data.highScores = highScores;
        data.coins += coins;

        SaveSystem.SavePlayer(data);
        QuitGame();
    }

    public IEnumerator GameOverMultiplayer() {
        multiplayerScoreList = new List<KeyValuePair<string, int>>();

        foreach (var player in PhotonNetwork.PlayerList) { 
            while (player.CustomProperties["Score"] == null) {
                yield return null;
            }
            int score = (int)player.CustomProperties["Score"];
            
            multiplayerScoreList.Add(new KeyValuePair<string, int>(player.NickName, score));

            // delete score from player's custom properties
            Hashtable hash = new Hashtable();
            hash.Add("Score", 0);
            player.SetCustomProperties(hash);
        }


        multiplayerScoreList.Sort(
            delegate (KeyValuePair<string, int> firstPair,
            KeyValuePair<string, int> nextPair) {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
        );

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("GameOverMultiplayer");
        }
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

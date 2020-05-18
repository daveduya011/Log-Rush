using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string username = "";

    public string[] highScoreNames;
    public int[] highScores;
    public int coins;
    public int level;
    public float levelProgress;

    public List<string> ownedCharactersId = new List<string>(); 
    public List<string> ownedLogsId = new List<string>(); 
    public List<string> ownedRiversId = new List<string>();

    public string equippedCharacterId = "";
    public string equippedLogId = "";
    public string equippedRiverId = "";

    public PlayerData(PlayerData data) {
        highScoreNames = data.highScoreNames;
        highScores = data.highScores;
        coins = data.coins;

        ownedCharactersId = data.ownedCharactersId;
        ownedLogsId = data.ownedLogsId;
        ownedRiversId = data.ownedRiversId;

        equippedCharacterId = data.equippedCharacterId;
        equippedLogId = data.equippedLogId;
        equippedRiverId = data.equippedRiverId;

        username = data.username;
        level = data.level;
    }
    public PlayerData() {
        highScoreNames = new string[3];
        highScores = new int[3];
        coins = 0;
        level = 1;
    }
}

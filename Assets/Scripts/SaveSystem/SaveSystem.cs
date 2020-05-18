using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveSettings(Settings settings) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetAppPath("settings");
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(settings);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void SavePlayer(PlayerData playerData) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetAppPath("player");
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(playerData);

        formatter.Serialize(stream, data);
        stream.Close();

        GameManager.Instance.RefreshPlayerData(playerData);
    }


    public static SettingsData LoadSettings() {
        string path = GetAppPath("settings");
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            return data;
        }
        else {
            SettingsData data = new SettingsData();
            return data;
        }
    }

    public static PlayerData LoadPlayer() {
        string path = GetAppPath("player");
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else {
            PlayerData data = new PlayerData();
            return data;
        }
    }

    private static string GetAppPath(string fileName) {
        return Application.persistentDataPath + "/" + fileName + ".dat";
    }
}

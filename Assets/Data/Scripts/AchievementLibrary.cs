using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AchievementLibrary", menuName = "Achievement Library", order = 1)]
public class AchievementLibrary : ScriptableObject
{
    public List<Achievement> achievements = new List<Achievement>();
}

[System.Serializable]
public class MyEvent : UnityEvent<string, GameObject> { }

[System.Serializable]
public class Achievement
{
    [HideInInspector]
    public string id;
    public string title;
    public string description;
    public Sprite image;

    public MyEvent yeah;
    //id = Guid.NewGuid().ToString();
}

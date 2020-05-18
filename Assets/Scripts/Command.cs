using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Command
{
    public enum Type { ICON, TEXT }
    public enum Gesture { UP, DOWN, LEFT, RIGHT, TAP}

    public Type type = Type.ICON;
    public Gesture gesture = Gesture.UP;
    public Color color = Color.white;
    public string word = "";
    public int times;
    public bool isDone;

}

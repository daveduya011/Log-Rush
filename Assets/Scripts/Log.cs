using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    public bool isActive;
    public bool isDoNothing;
    public bool isNegative;

    public enum NegativePrefix { NOT, DO_NOT_SWIPE }
    public NegativePrefix negativePrefix = NegativePrefix.NOT;

    public Command[] commands;
    public Color correctColor = Color.white;
    public Color incorrectColor = Color.white;

    [HideInInspector]
    public Command currentCommand;
    public enum Conjunction { OR, THEN }
    public Conjunction conjunction = Conjunction.OR;
    public bool isShake;


    public Animator animator;


    public int currentIndex = 0;
    public int inputCounter = 0;
    private int numOfConjunctions;

    public SkinLibrary skinLibrary;
    public Text text;

    public SpriteRenderer crocodileFront;
    public SpriteRenderer crocodileBack;

    // Start is called before the first frame update
    void Start() {
        LoadDefaultLogSprite();

        isDoNothing = RandomGenerator.GenerateRandomDoNothing();

        if (isDoNothing) {
            text.text = "do nothing";
            return;
        }

        GenerateCommand();
        currentCommand = commands[0];
        CreateText();

    }

    private void LoadDefaultLogSprite() {
        PlayerData data = GameManager.Instance.GetPlayerData();

        if (data.equippedLogId == null)
            return;

        Color defaultColor = text.color;
        Sprite defaultFrontCrocodile = crocodileFront.sprite;
        Sprite defaultBackCrocodile = crocodileBack.sprite;
        foreach (SkinAsset asset in skinLibrary.logAssets) {
            if (data.equippedLogId.Equals(asset.id)) {
                text.color = ((LogAsset)asset).textColor;
                crocodileFront.sprite = ((LogAsset)asset).crocodileImageFront;
                crocodileBack.sprite = ((LogAsset)asset).crocodileImageBack;
                return;
            }

            if (asset.isEquipped) {
                defaultColor = ((LogAsset)asset).textColor;
                defaultFrontCrocodile = ((LogAsset)asset).crocodileImageFront;
                defaultBackCrocodile = ((LogAsset)asset).crocodileImageBack;
            }
        }

        text.color = defaultColor;
        crocodileFront.sprite = defaultFrontCrocodile;
        crocodileBack.sprite = defaultBackCrocodile;
    }

    private void GenerateCommand() {
        

        isNegative = RandomGenerator.GenerateRandomNegative();

        int numOfCommands = isNegative ? 1 : RandomGenerator.GenerateRandomCommand();

        commands = new Command[numOfCommands];
       
        int numOfTypes = Enum.GetNames(typeof(Command.Type)).Length;
        int numOfGestures = Enum.GetNames(typeof(Command.Gesture)).Length;
        int numOfNegativePrefix = Enum.GetNames(typeof(NegativePrefix)).Length;
        numOfConjunctions = Enum.GetNames(typeof(Conjunction)).Length;
        conjunction = (Conjunction)RandomGenerator.GenerateRandom(numOfConjunctions);

        text.text = "";
        for (int i = 0; i < commands.Length; i++) {
            commands[i] = new Command();

            int randType = RandomGenerator.GenerateRandom(numOfTypes);
            int randGesture = isNegative ? RandomGenerator.GenerateRandom(numOfGestures - 1) : RandomGenerator.GenerateRandom(numOfGestures);
            int randTimes = RandomGenerator.GenerateRandomTimes();

            commands[i].type = (Command.Type)randType;
            commands[i].gesture = (Command.Gesture)randGesture;
            commands[i].times = randTimes;

            string prefix = "";
            if (isNegative) {
                int randPrefix = RandomGenerator.GenerateRandom(numOfNegativePrefix);
                negativePrefix = (NegativePrefix)randPrefix;
                commands[i].times = 1;
                prefix = RemoveWhitespace(negativePrefix.ToString()).ToLower() + " ";
            }


            string suffix = commands[i].times > 1 ? " " + commands[i].times + "x" : "";
            string text = commands[i].gesture.ToString();
            if (commands[i].type == Command.Type.ICON) {
                switch(commands[i].gesture) {
                    case Command.Gesture.LEFT:
                        text = " \u2190 ";
                        break;
                    case Command.Gesture.UP:
                        text = " \u2191 ";
                        break;
                    case Command.Gesture.RIGHT:
                        text = " \u2192 ";
                        break;
                    case Command.Gesture.DOWN:
                        text = " \u2193 ";
                        break;
                    default:
                        break;
                }
            }
            commands[i].word = prefix + text.ToLower() + suffix;
            commands[i].color = this.text.color;
        }

    }

    private void CreateText() {
        text.text = "";
        for (int i = 0; i < commands.Length; i++) {
            string conjunction = "<size=12>" + this.conjunction.ToString() + " </size>";
            if (i == 0)
                conjunction = "";
            string word = "<color=#" + ColorUtility.ToHtmlStringRGB(commands[i].color) + ">" + conjunction.ToLower() + commands[i].word + "</color>";

            if (i > 0) {
                word = " " + word;
            }
            text.text += word;
        }
    }


    public void GoToNextCommand() {
        inputCounter = 0;
        currentIndex++;
        currentCommand = commands[currentIndex];
        isActive = true;
    }

    public void OnCorrect() {
        if (isDoNothing) {
            text.color = correctColor;
            return;
        }
        currentCommand.color = correctColor;
        CreateText();
        
    }

    public void OnIncorrect() {
        if (isDoNothing) {
            text.color = incorrectColor;
            return;
        }
        currentCommand.color = incorrectColor;
        CreateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string RemoveWhitespace(string str) {
        return string.Join(" ", str.Split('_'));
    }

}

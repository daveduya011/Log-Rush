using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UsernameInput : MonoBehaviour
{
    public InputField inputField;
    private bool isSubmitClicked;
    public Button submitButton;
    public Text warningText;

    enum TextException
    {
        SHORT, SPECIAL_CHARS, VALID, WHITE_SPACE
    }
    private const string INVALID_PATTERN = "[^\\w$&^*. -]";
    private int minimumCharacters = 3;
    

    void Start() {
        submitButton.onClick.AddListener(() => isSubmitClicked = true);
        inputField.onValueChanged.AddListener(ValidateCharacters);
        warningText.text = "";
        submitButton.interactable = false;
    }

    private void ValidateCharacters(string text) { 
        // replace the double spaces
        string newText = new Regex("[ ]{2,}").Replace(text, " ").TrimStart();

        switch(CheckValid(newText)) {
            case TextException.SHORT:
                warningText.text = "Minimum characters should be " + minimumCharacters;
                submitButton.interactable = false;
                break;
            case TextException.SPECIAL_CHARS:
                warningText.text = "Special characters are not allowed";
                submitButton.interactable = false;
                break;
            case TextException.VALID:
                warningText.text = "";
                submitButton.interactable = true;
                break;
        }
        inputField.text = newText;
    }

    private TextException CheckValid(string text) {
        // Replace any double space character with a space
        if (!string.IsNullOrWhiteSpace(text)) {
             if (text.Length < minimumCharacters) {
                return TextException.SHORT;
            }  else if (new Regex(INVALID_PATTERN).IsMatch(text) == true) {
                return TextException.SPECIAL_CHARS;
            } else {
                return TextException.VALID;
            }

        } else {
            return TextException.WHITE_SPACE;
        }
    }

    public async Task<string> RequestUsernameInput(string id, string idToken) {
        isSubmitClicked = false;
        gameObject.SetActive(true);
        while (!isSubmitClicked)
            await Task.Yield();
        gameObject.SetActive(false);
        return inputField.text;
    }
}

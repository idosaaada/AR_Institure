using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Struct to define the properties of a screen
[System.Serializable]
public struct ScreenProperties
{
    public bool isPopUp; // Determines if this screen has a pop-up
    public string HeaderText; // The header text to be displayed
    public string ContentText; // The content text to be displayed
}

// Class that handles the behavior of a screen item
public class ScreenItem : MonoBehaviour
{
    public GameObject NextButton; // Reference to the "Next" button UI element
    public PopUp PopUp; // Reference to a PopUp component for displaying pop-up messages
    public bool isNextButton; // Flag to determine if the "Next" button should be displayed
    public GameObject InfoBtn; // Reference to the "Info" button UI element
    public bool isInfoButton = true; // Flag to determine if the "Info" button should be displayed
    public bool isProgressBar = true; // Flag to determine if the progress bar should be shown

    public ScreenProperties BeginScreen; // Properties for the screen when it begins
    public ScreenProperties EndScreen; // Properties for the screen when it ends

    // Called when the object becomes enabled and active
    private void OnEnable()
    {
        OnScreenBegin(); // Initialize the screen's beginning state
        FindObjectOfType<GameManager>().ToggleProgressBar(isProgressBar); // Toggle progress bar visibility based on the flag
    }

    // Handles the actions that should occur when the screen begins
    public void OnScreenBegin()
    {
        ToggleNextButton(isNextButton); // Show or hide the "Next" button based on the flag
        PopUp = FindObjectOfType<PopUp>(); // Find the PopUp component in the scene
        PopUp.SetPopUpTexts(BeginScreen.HeaderText, BeginScreen.ContentText); // Set the header and content for the pop-up
        PopUp.TogglePopUp(BeginScreen.isPopUp); // Show or hide the pop-up based on the screen's properties
        InfoBtn.SetActive(isInfoButton); // Show or hide the "Info" button based on the flag

        if (LanguageController.instance != null)
            LanguageController.instance.TranslateAllText(); // Translate all TextMeshProUGUI components in the scene
    }

    // Handles the actions that should occur when the screen ends
    public void OnScreenEnded()
    {
        PopUp = FindObjectOfType<PopUp>(); // Find the PopUp component in the scene
        PopUp.SetPopUpTexts(EndScreen.HeaderText, EndScreen.ContentText); // Set the header and content for the pop-up
        PopUp.TogglePopUp(EndScreen.isPopUp); // Show or hide the pop-up based on the screen's end properties
        ToggleNextButton(true); // Ensure the "Next" button is shown when the screen ends

        if (LanguageController.instance != null)
            LanguageController.instance.TranslateAllText(); // Translate all TextMeshProUGUI components in the scene

    }

    // Toggles the visibility of the "Next" button
    public void ToggleNextButton(bool Toggle)
    {
        NextButton.SetActive(Toggle); // Activate or deactivate the "Next" button based on the toggle value
    }
}
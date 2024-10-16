using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this line to use TextMeshProUGUI

public class ButtonTextDisplay : MonoBehaviour
{
    // Variables to store the buttons and text fields in the game
    public Button button1;
    public Button button2;
    public Button button3;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;

    void Start()
    {
        // Initially hide all text elements
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        text3.gameObject.SetActive(false);

        // Assign event listeners to each button
        // When clicked, they will display the corresponding text
        button1.onClick.AddListener(() => DisplayText(text1));
        button2.onClick.AddListener(() => DisplayText(text2));
        button3.onClick.AddListener(() => DisplayText(text3));
    }

    // Function to show the text when the associated button is clicked
    void DisplayText(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true); // Make the selected text visible
    }
}

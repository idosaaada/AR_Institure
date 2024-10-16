using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUp : MonoBehaviour
{
    public Transform holder; // The UI element that contains the pop-up content
    public TextMeshProUGUI Header; // Text component for the pop-up header
    public TextMeshProUGUI Content; // Text component for the pop-up content

    // Method to show or hide the pop-up
    public void TogglePopUp(bool toggle)
    {
        holder.gameObject.SetActive(toggle); // Activates or deactivates the holder based on the toggle value
    }

    // Method to set the header and content text of the pop-up
    public void SetPopUpTexts(string header, string content)
    {
        Header.text = header; // Set the header text
        Content.text = content; // Set the content text
    }
}

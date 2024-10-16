using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class handles displaying an avatar image in a Unity UI
public class ShowAvatar : MonoBehaviour
{
    // Property to get the Image component of the first child object
    private Image myAvatar => GetComponentInChildren<Image>();

    // Property to find and get the DataManager in the scene
    DataManager dataManager => FindObjectOfType<DataManager>();

    // This method is called every time the GameObject this script is attached to becomes enabled
    private void OnEnable()
    {
        // Check if dataManager exists, has a valid avatar, and myAvatar component exists
        if (dataManager != null && dataManager.GetAvatar() != null
            && myAvatar != null)
        {
            // Set the sprite of myAvatar to the avatar from dataManager
            myAvatar.sprite = dataManager.GetAvatar();
        }
    }
}

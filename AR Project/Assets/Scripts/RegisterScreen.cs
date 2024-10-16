using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RegisterScreen : MonoBehaviour
{
    public TMP_InputField groupName; // Input field for the group name
    public TMP_InputField playerName; // Input field for the player's name
    public Image avatar; // The avatar selected by the user
    public Image defaultAvatar; // The default avatar image
    public DataManager dataManager => FindObjectOfType<DataManager>(); // Reference to DataManager (using property with FindObjectOfType)

    private void Start()
    {
        SetAvatar(defaultAvatar); // Set the default avatar at the start
    }

    // Method to save the group data into DataManager
    public void SaveGroupData()
    {
        // Save the group and player names, as well as the avatar image, to DataManager
        dataManager.groupNameInput = groupName.text;
        dataManager.playerNameInput = playerName.text;
        dataManager.avatar = avatar;

        // Save the group data through DataManager
        dataManager.SaveGroup();

        // Signal that the screen has ended using the ScreenItem component
        GetComponent<ScreenItem>().OnScreenEnded();
    }

    // Method to set a new avatar image
    public void SetAvatar(Image _avatar)
    {
        avatar = _avatar;
    }
}
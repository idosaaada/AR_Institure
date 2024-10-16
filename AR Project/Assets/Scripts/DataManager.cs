using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Represents a group of players
[Serializable]
public class GroupData
{
    public string groupName; // Name of the group
    public List<string> playerNames; // List of player names in the group
    public string avatar; // The avatar associated with the group

    // Constructor initializes the playerNames list
    public GroupData()
    {
        playerNames = new List<string>();
    }
}

// Represents the overall game data, containing multiple groups
[Serializable]
public class GameData
{
    public List<GroupData> groups; // List of all groups in the game

    // Constructor initializes the groups list
    public GameData()
    {
        groups = new List<GroupData>();
    }
}

// Manages data operations for the game
public class DataManager : MonoBehaviour
{
    public string groupNameInput; // Stores the input for group name
    public string playerNameInput; // Stores the input for player name
    public Image avatar; // Reference to the avatar image
    private string gameDataFilePath; // Path to save/load game data
    GroupData group; // Currently loaded group

    // Initializes the file path for game data
    private void Start()
    {
        // Save the path where the game data will be stored
        gameDataFilePath = Application.persistentDataPath + "/gamedata.json";
    }

    // Saves a new group to the game data
    public void SaveGroup()
    {
        // Load existing game data or create a new one
        GameData gameData = LoadData();

        // Create a new GroupData instance and set its values
        GroupData groupData = new GroupData();
        groupData.groupName = groupNameInput; // Assign group name input
        groupData.avatar = avatar.name; // Set the avatar name
        groupData.playerNames.Add(playerNameInput); // Add the player name

        // Add the new group to the game data
        gameData.groups.Add(groupData);

        // Convert game data to JSON format and write it to the file
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(gameDataFilePath, json);
    }

    // Loads game data from file, or creates new data if file doesn't exist
    public GameData LoadData()
    {
        // Check if the game data file exists
        if (File.Exists(gameDataFilePath))
        {
            // Read the JSON content from the file
            string json = File.ReadAllText(gameDataFilePath);

            // Convert the JSON back to a GameData object and return it
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            // Return a new GameData object if no file is found
            return new GameData();
        }
    }

    // Loads a specific group by name
    public GroupData LoadGroup(string groupName)
    {
        // Load all game data
        GameData gameData = LoadData();

        // Search for the group that matches the given name
        foreach (GroupData group in gameData.groups)
        {
            if (group.groupName == groupName)
            {
                return group; // Return the group if found
            }
        }

        // If the group is not found, log a warning message
        Debug.LogWarning("Group not found: " + groupName);
        return null; // Return null if not found
    }

    // Loads and displays group data for debugging
    public void LoadGroupData()
    {
        // Load the group based on the input group name
        group = LoadGroup(groupNameInput);

        // If the group exists, display its data in the console
        if (group != null)
        {
            Debug.Log("Group Name: " + group.groupName);
            Debug.Log("Avatar: " + group.avatar);
            foreach (string playerName in group.playerNames)
            {
                Debug.Log("Player Name: " + playerName);
            }
        }
    }

    // Returns the sprite of the current avatar
    public Sprite GetAvatar()
    {
        // Returns the sprite (image) of the avatar currently selected
        return avatar.sprite;
    }
}
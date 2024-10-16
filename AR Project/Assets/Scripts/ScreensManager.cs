using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class manages multiple screens in a Unity application
public class ScreensManager : MonoBehaviour
{
    public Transform ScreensHolder; // Parent transform containing all screen objects
    public List<Transform> Screens; // List to store individual screen transforms
    public int screenIndex = 0; // Index of the currently active screen

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Populate the Screens list with all child objects of ScreensHolder
        foreach (Transform child in ScreensHolder)
        {
            Screens.Add(child);
        }

        // Deactivate all screens
        foreach (Transform screen in Screens)
        {
            screen.gameObject.SetActive(false);
        }

        // Activate the first screen (index 0)
        Screens[0].gameObject.SetActive(true);
    }

    // Method to move to the next screen
    public void NextScreen()
    {
        // Check if there's a next screen available
        if (screenIndex < Screens.Count ) 
        {
            Screens[screenIndex].gameObject.SetActive(false); // Deactivate current screen
            screenIndex++; // Move to next screen
            Screens[screenIndex].gameObject.SetActive(true); // Activate next screen
            FindObjectOfType<GameManager>().CalculateScore(1); // Update score (increment)
        }
    }

    // Method to move to the previous screen
    public void PrevScreen()
    {
        // Check if there's a previous screen available
        if (screenIndex >= 0) 
        {
            Screens[screenIndex].gameObject.SetActive(false); // Deactivate current screen
            screenIndex--; // Move to previous screen
            Screens[screenIndex].gameObject.SetActive(true); // Activate previous screen
            FindObjectOfType<GameManager>().CalculateScore(-1); // Update score (decrement)
        }
    }
}

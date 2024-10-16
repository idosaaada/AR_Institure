using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform mainCam;
    public ScreensManager screensManager; // Reference to the screen manager for managing screens
    public GameObject SettingsScreen; // Reference to the settings screen UI

    public Transform progressHolder; // Holder for the progress UI elements
    public Image ProgressBar; // Image representing the progress bar

    public TextMeshProUGUI scoreText; // Text UI element to display the score
    public int maxScreens; // Maximum number of screens 
    public int currentScreens; // Current screen (or stage) the player is on
    public int score = 0; // Player's score initialized to 0

    [SerializeField] private Slider volumeSlider; // Reference to the UI Slider for volume control
    [SerializeField] private List<AudioSource> audioSources; // List of AudioSources to adjust volume

    // Start is called before the first frame update
    void Start()
    {
        maxScreens = screensManager.Screens.Count-1; // Get the total number of screens from the screen manager
        scoreText.text = score.ToString(); // Display the initial score

        // Initialize the volume slider with the average volume of all audio sources
        if (volumeSlider != null && audioSources.Count > 0)
        {
            volumeSlider.value = GetAverageVolume(); // Set slider to average volume
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged); // Add listener to handle volume changes
        }
    }

    // Called when the volume slider value changes
    private void OnVolumeChanged(float volume)
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume; // Update the volume of each audio source
            }
        }
    }

    // Calculate the average volume of all the audio sources
    private float GetAverageVolume()
    {
        float totalVolume = 0f; // Total volume of all audio sources
        int validAudioSources = 0; // Count of valid audio sources

        // Iterate through all audio sources and sum their volumes
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null)
            {
                totalVolume += audioSource.volume;
                validAudioSources++;
            }
        }

        // Return the average volume or 0.5 if no valid audio sources found
        return validAudioSources > 0 ? totalVolume / validAudioSources : 0.5f;
    }

    // Calculate and update the player's score
    public void CalculateScore(int amount)
    {
        currentScreens += amount; // Add the amount (usually +1) to the current screen count

        if (currentScreens > maxScreens)
        {
            currentScreens = maxScreens; // Ensure the current screen does not exceed the maximum
        }

        // Calculate the score as a percentage of completed screens
        score = (currentScreens * 100) / maxScreens;
        scoreText.text = "Score: " + score.ToString(); // Update the score text UI

        // Normalize the score to a value between 0 and 1 for the progress bar
        float normalizedScore = (float)currentScreens / maxScreens;
        ProgressBar.fillAmount = normalizedScore; // Update the progress bar fill amount
    }

    // Exits the application
    public void ExitApp()
    {
        Application.Quit(); // Closes the application
    }

    // Toggles the visibility of the progress bar
    public void ToggleProgressBar(bool toggle)
    {
        progressHolder.gameObject.SetActive(toggle); // Show or hide the progress bar based on the toggle value
    }
}

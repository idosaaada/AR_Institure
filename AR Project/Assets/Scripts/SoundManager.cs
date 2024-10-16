using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Singleton instance of the SoundManager

    public AudioSource audioSource; // The AudioSource used to play sounds
    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>(); // Dictionary to store sounds by name

    private void Awake()
    {
        // Ensure there is only one instance of the SoundManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist the SoundManager across different scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        // Load all AudioClip resources from the "AudioClips" folder in Resources
        AudioClip[] tempClips = Resources.LoadAll<AudioClip>("AudioClips");

        // Add each clip to the dictionary with its name as the key
        foreach (AudioClip clip in tempClips)
        {
            sounds.Add(clip.name, clip);
        }
    }

    // Method to play a sound by its name
    public void PlaySound(string soundName)
    {
        // Play the sound as a one-shot clip using the audio source
        audioSource.PlayOneShot(sounds[soundName]);
    }
}

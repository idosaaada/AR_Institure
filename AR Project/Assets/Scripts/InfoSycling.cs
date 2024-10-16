using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class handles cycling through information or screens in Unity
public class InfoSycling : MonoBehaviour
{
    // Index to keep track of the current information or screen
    public int index = 0;

    // Method called when the object is clicked or interaction occurs
    public void OnClick()
    {
        // Increment the index
        index++;

        // Check if we've reached or exceeded the third item (index 2)
        if (index >= 3)
        {
            // If so, call the OnScreenEnded method of the ScreenItem component
            GetComponent<ScreenItem>().OnScreenEnded();
        }
    }
}

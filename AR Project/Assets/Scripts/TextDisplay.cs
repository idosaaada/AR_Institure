using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiTextDisplayTMP : MonoBehaviour
{
    public GameObject[] tips; // Array of GameObjects representing different tips to display
    public int index = 0; // Index to track the currently displayed tip

    // Function to display the next tip in the array
    public void NextTip()
    {
        // If the current index is less than the total number of tips, show the next one
        if (index < tips.Length - 1)
        {
            index++; // Increment the index to move to the next tip
            tips[index].gameObject.SetActive(true); // Activate the next tip GameObject
        }
        else
        {
            // If there are no more tips, trigger the end of the screen
            GetComponent<ScreenItem>().OnScreenEnded();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CycleGame : MonoBehaviour
{
    public Transform objectsParent; // Parent object containing the draggable images
    public DragHandler[] validImges; // Array to hold all valid DragHandler objects (draggable images)
    public int completeCount = 0; // Counter to track the number of completed images

    // Start is called before the first frame update
    private void Start()
    {
        // Find all DragHandler components (draggable images) that are children of objectsParent
        validImges = objectsParent.GetComponentsInChildren<DragHandler>();
    }

    // Method to check if all images have been completed (placed in their correct position)
    public void CheckForEndMission()
    {
        completeCount++; // Increment the counter when an image is placed correctly

        // Check if the count equals the total number of images (all images are correctly placed)
        if (completeCount == validImges.Length)
        {
            Debug.Log("Mission Complete!"); // Log a message indicating the mission is complete

            // Notify the ScreenItem component that the screen has ended
            GetComponent<ScreenItem>().OnScreenEnded();
        }
    }
}

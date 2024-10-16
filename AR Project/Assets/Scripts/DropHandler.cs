using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This class handles the drop functionality for UI elements in Unity
public class DropHandler : MonoBehaviour, IDropHandler
{
    // This method is called when a draggable object is dropped onto this object
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        // Check if there's a valid object being dragged
        if (eventData.pointerDrag != null)
        {
            // Get the DragHandler component from the dragged object
            DragHandler drag = eventData.pointerDrag.GetComponent<DragHandler>();
            Debug.Log("obj" + drag.isValid);

            // Check if the tags of the dragged object and this object match
            if (gameObject.tag == eventData.pointerDrag.gameObject.tag)
            {
                // If tags match, consider it a valid drop
                drag.GetComponent<Image>().raycastTarget = false; // Disable further interaction with the dropped object
                drag.complete = true; // Mark the drag operation as complete
                SoundManager.instance.PlaySound("Click"); // Play a sound effect
                FindObjectOfType<CycleGame>().CheckForEndMission(); // Check if the mission is complete
            }
            else
            {
                // If tags don't match, consider it an invalid drop
                drag.OnNotValid(); // Reset the dragged object to its original position
                SoundManager.instance.PlaySound("Click"); // Play a sound effect
            }

            // Add additional logic here, like snapping to a specific position
        }
    }
}

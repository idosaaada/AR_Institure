using UnityEngine;
using UnityEngine.EventSystems;

// This class handles drag and drop functionality for UI elements in Unity
public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    private Vector2 originalPos;
    public bool isValid;
    public bool complete;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        originalPos = rectTransform.anchoredPosition; // Store the original position of the UI element
    }

    // Called when a pointer down event occurs
    public void OnPointerDown(PointerEventData eventData)
    {
        // This method is currently empty but can be used to add functionality when the pointer is pressed down
    }

    // Called when a drag event begins
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Make the UI element semi-transparent while dragging
        canvasGroup.blocksRaycasts = false; // Disable raycasting while dragging to allow detection of elements underneath
    }

    // Called every frame while dragging
    public void OnDrag(PointerEventData eventData)
    {
        // Update the position of the UI element based on the drag delta
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // Called when a drag event ends
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Restore the UI element opacity
        canvasGroup.blocksRaycasts = true; // Enable raycasting after dragging
    }

    // Called when the drag is not valid (e.g., dropped in an invalid area)
    public void OnNotValid()
    {
        rectTransform.anchoredPosition = originalPos; // Reset the UI element to its original position
    }
}
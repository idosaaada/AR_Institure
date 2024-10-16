using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // This function handles the exit logic when the button is clicked
    public void ExitGame()
    {
        // Check if the game is running in the Unity Editor
#if UNITY_EDITOR
        // Stop playing the game in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If the game is running as a standalone build, quit the application
        Application.Quit();
#endif
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerQuick : MonoBehaviour
{
    public Transform holder; // The parent UI element where targets will be spawned
    public GameObject targetPrefab; // Prefab of the target to spawn
    public QuickItem[] correctSprites; // Array of correct sprites (with associated titles)
    public QuickItem[] wrongSprites; // Array of incorrect sprites (with associated titles)
    public TextMeshProUGUI scoreText; // Text element displaying the current score
    public float spawnInterval = 3f; // Time between spawns
    public float displayDuration = 2f; // Time to display each target before removing it
    GameObject Target; // Reference to the currently spawned target
    private int score = 0; // Player's score
    public bool isCorrect; // Boolean to track if the current target is correct

    void Start()
    {
        StartCoroutine(SpawnTargets()); // Start spawning targets
        scoreText.text = "Points: 0 "; // Initialize score display
    }

    // Coroutine to continuously spawn targets
    IEnumerator SpawnTargets()
    {
        while (true)
        {
            // Generate random position for the target within a specified range
            Vector2 spawnPosition = new Vector2(Random.Range(-400f, 400f), Random.Range(-150f, 35f));
            Debug.Log(spawnPosition);

            if (Target == null) // If there is no active target
            {
                int rand = Random.Range(0, 2); // Randomly choose whether to spawn a correct or incorrect target
                Sprite newSprite = null;
                QuickItem newItem = new QuickItem();

                if (rand == 0) // Choose a correct sprite
                {
                    newItem = correctSprites[Random.Range(0, correctSprites.Length)];
                    newSprite = newItem.img;
                    isCorrect = true; // Mark the target as correct
                }
                else // Choose an incorrect sprite
                {
                    newItem = wrongSprites[Random.Range(0, wrongSprites.Length)];
                    newSprite = newItem.img;
                    isCorrect = false; // Mark the target as incorrect
                }

                // Instantiate the target and set its text and image
                GameObject newTarget = Instantiate(targetPrefab, holder);
                newTarget.GetComponentInChildren<TextMeshProUGUI>().text = newItem.imgTitle;
                Target = newTarget;
                newTarget.GetComponent<Image>().sprite = newSprite;
                newTarget.transform.SetParent(holder.transform); // Set its parent
                newTarget.GetComponent<RectTransform>().anchoredPosition = spawnPosition; // Set its position
            }

            clicked = false; // Reset clicked state for new target

            // Attach click listener to the target's button
            Button button = Target.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnTargetClicked(Target));
            }
            else
            {
                Debug.LogWarning("Button component is missing on the target prefab.");
            }

            // Wait for the target to be displayed for the specified duration
            yield return new WaitForSeconds(displayDuration);

            if (Target != null && !clicked) // If the target wasn't clicked in time, destroy it
            {
                Destroy(Target);
                Target = null;
            }

            // Wait for the remainder of the interval before spawning the next target
            yield return new WaitForSeconds(spawnInterval - displayDuration);
        }
    }

    bool clicked; // Track if the target was clicked
    void OnTargetClicked(GameObject target)
    {
        if (clicked) return; // Prevent multiple clicks
        clicked = true;

        // Update score based on whether the clicked target was correct or incorrect
        if (isCorrect)
        {
            ++score;
            SoundManager.instance.PlaySound("correct choice");
        }
        else
        {
            --score;
            SoundManager.instance.PlaySound("error");
        }

        // If score reaches 5, end the game
        if (score == 5)
        {
            Debug.Log("Game finished!!");
            GetComponent<ScreenItem>().OnScreenEnded(); // Signal the end of the screen
            SoundManager.instance.PlaySound("Victory Sound");
            holder.gameObject.SetActive(false); // Hide the holder containing the targets
        }

        // Update the score display
        scoreText.text = "Points: " + score.ToString();

        // Destroy the clicked target
        Destroy(target);
        Target = null;
    }
}

// Struct representing an item with a title and associated sprite
[System.Serializable]
public struct QuickItem
{
    public string imgTitle; // Title of the image (e.g., concept)
    public Sprite img; // The image (sprite) itself
}
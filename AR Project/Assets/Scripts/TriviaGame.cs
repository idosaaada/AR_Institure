using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

// Serializable class to hold individual trivia questions
[System.Serializable]
public class TriviaQuestion
{
    public string question; // The trivia question text
    public string[] answers; // Array of possible answers
    public int correctAnswerIndex; // Index of the correct answer in the answers array
}

// Serializable class to hold a collection of trivia questions
[System.Serializable]
public class TriviaData
{
    public List<TriviaQuestion> questions; // List of trivia questions
}

public class TriviaGame : MonoBehaviour
{
    public TriviaData triviaData; // Data container for the trivia questions
    public TextMeshProUGUI questionText; // UI element to display the question text
    public Button[] answerButtons; // UI elements for the answer buttons
    public TextMeshProUGUI timerText; // UI element to display the countdown timer
    public TextMeshProUGUI feedbackText; // UI element for feedback (correct/incorrect)

    public List<TriviaQuestion> questions; // List of trivia questions used in the game
    private int currentQuestionIndex = 0; // Index of the current question
    private float timeRemaining = 30f; // Timer for each question
    private bool isAnswering = true; // Whether the player is still answering the current question

    // Start is called before the first frame update
    void Start()
    {
        LoadQuestionsFromJSON(); // Load questions from the JSON file
        ShuffleQuestions(); // Shuffle the list of questions
        DisplayQuestion(); // Display the first question
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnswering)
        {
            UpdateTimer(); // Continuously update the timer while answering
        }
    }

    public TextAsset HebrewJson, EnglishJson, ArabicJson;
    public TextAsset JsonTxt; // Holds the JSON file with trivia questions

    // Loads questions from a JSON file and deserializes them into TriviaData
    void LoadQuestionsFromJSON()
    {
        if(LanguageController.instance.currentLanguage == Language.English)
        {
            JsonTxt = EnglishJson;
        }
        else if (LanguageController.instance.currentLanguage == Language.Arabic)
        {
            JsonTxt = ArabicJson;
        }
        else if (LanguageController.instance.currentLanguage == Language.Hebrew)
        {
            JsonTxt = HebrewJson;
        }

        if (JsonTxt != null)
        {
            // Load JSON data and convert it into a TriviaData object
            string dataAsJson = JsonTxt.text;
            triviaData = JsonUtility.FromJson<TriviaData>(dataAsJson);
            Debug.Log("JSON Content: " + dataAsJson);

            // Assign the list of questions from the loaded data
            questions = triviaData.questions;
            Debug.Log(questions.Count);
        }
        else
        {
            Debug.LogError("JsonTxt is null. Please assign the TextAsset in the Inspector.");
        }
    }

    // Shuffles the list of questions using Fisher-Yates shuffle algorithm
    void ShuffleQuestions()
    {
        System.Random rng = new System.Random();
        int n = questions.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            TriviaQuestion value = questions[k];
            questions[k] = questions[n];
            questions[n] = value;
        }
    }

    // Displays the current question and assigns answers to buttons
    void DisplayQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndGame(); // End the game if no more questions
            return;
        }

        isAnswering = true; // Allow player to answer the question
        timeRemaining = 30f; // Reset timer for the new question

        // Get the current question and update the question text UI
        TriviaQuestion currentQuestion = questions[currentQuestionIndex];
        questionText.text = currentQuestion.question;
        if (LanguageController.instance.currentLanguage == Language.English)
        {
            questionText.font = LanguageController.instance.otherFont;
            questionText.isRightToLeftText = false;

        }
        else if (LanguageController.instance.currentLanguage == Language.Arabic)
        {
            questionText.font = LanguageController.instance.arabicFont;
            questionText.isRightToLeftText = true;

        }
        else if (LanguageController.instance.currentLanguage == Language.Hebrew)
        {
            questionText.isRightToLeftText = true;
            questionText.font = LanguageController.instance.otherFont;

        }
        // Set the text of each answer button and assign click listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capture the index for the button listener
            TextMeshProUGUI txt = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            txt.text = currentQuestion.answers[i];
            answerButtons[i].onClick.RemoveAllListeners(); // Remove previous listeners
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index)); // Add listener to check answer

            if (LanguageController.instance.currentLanguage == Language.English)
            {
                txt.font = LanguageController.instance.otherFont;
                txt.isRightToLeftText = false;

            }
            else if (LanguageController.instance.currentLanguage == Language.Arabic)
            {
                txt.font = LanguageController.instance.arabicFont;
                txt.isRightToLeftText = true;

            }
            else if (LanguageController.instance.currentLanguage == Language.Hebrew)
            {
                txt.isRightToLeftText = true;
                txt.font = LanguageController.instance.otherFont;

            }
        }

        feedbackText.gameObject.SetActive(false); // Hide feedback text initially
    }

    // Checks if the selected answer is correct and updates the UI accordingly
    void CheckAnswer(int selectedIndex)
    {
        isAnswering = false;// Stop the player from answering
        TriviaQuestion currentQuestion = questions[currentQuestionIndex];

        // If the selected answer is correct
        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            feedbackText.text = "תשובה נכונה, כל הכבוד!";// Show positive feedback
            answerButtons[selectedIndex].GetComponent<Image>().color = Color.green; // Highlight correct answer in green
            SoundManager.instance.PlaySound("correct choice"); // Play correct answer sound
        }
        else
        {
            feedbackText.text = "תשובה לא נכונה";// Show negative feedback
            answerButtons[selectedIndex].GetComponent<Image>().color = Color.red; // Highlight wrong answer in red
            SoundManager.instance.PlaySound("error"); // Play error sound
        }

        feedbackText.gameObject.SetActive(true); // Show feedback text
        StartCoroutine(NextQuestionAfterDelay()); // Move to next question after a delay
    }

    // Waits for a few seconds before moving to the next question
    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        ResetButtonColors(); // Reset the colors of answer buttons
        currentQuestionIndex++; // Move to the next question
        DisplayQuestion(); // Display the next question
    }

    // Resets the colors of the answer buttons to their default (white)
    void ResetButtonColors()
    {
        foreach (Button button in answerButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }

    // Updates the timer and checks if time has run out
    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime; // Decrease remaining time
        timerText.text = timeRemaining.ToString("F0"); // Update timer UI

        if (timeRemaining <= 0)
        {
            isAnswering = false; // Stop the player from answering
            feedbackText.text = "הזמן נגמר!"; // Show "time's up" message
            feedbackText.gameObject.SetActive(true); // Display the feedback
            StartCoroutine(NextQuestionAfterDelay()); // Move to next question
        }
    }


    // Ends the game by hiding the answer buttons and stopping the timer
    void EndGame()
    {
        questionText.text = "המשחק הסתיים!";// Show "game over" message

        // Hide all answer buttons
        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Hide the timer UI
        timerText.gameObject.SetActive(false);

        // Call a method from another component (ScreenItem) to handle end-of-game logic
        GetComponent<ScreenItem>().OnScreenEnded();
    }
}
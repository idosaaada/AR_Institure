using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

// Class representing a card with a concept and explanation
[System.Serializable]
public class Card
{
    public string concept; // The concept side of the card
    public string explanation; // The explanation side of the card
}

// Class to hold a list of cards
[System.Serializable]
public class CardData
{
    public List<Card> cards; // List of all cards (concepts and explanations)
}

// Main class for managing the memory game
public class MemoryGame : MonoBehaviour
{
    public List<Button> cardButtons; // List of UI buttons representing cards
    private Button firstCard; // First card selected by the player
    private Button secondCard; // Second card selected by the player
    private bool canSelect = true; // Flag to control if cards can be selected
    public bool GameEnded; // Flag to indicate if the game has ended

    private CardData cardData; // Holds the data of the cards
    private Dictionary<Button, (string, bool)> cardContentDict; // Maps buttons to card content (content, isConcept)

    public TextAsset HebrewJson, EnglishJson, ArabicJson; // TextAssets for different language JSON files
    public TextAsset JsonTxt; // Reference to a TextAsset containing the selected JSON data

    // Start is called before the first frame update
    void Start()
    {
        LoadCardData(); // Load card data from a JSON file
        SetupCards(); // Set up the cards for the game
    }

    // Load card data from the JSON file based on the current language
    void LoadCardData()
    {
        // Choose the correct JSON file based on the current language
        if (LanguageController.instance.currentLanguage == Language.English)
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

        // Parse the JSON data and load it into cardData
        if (JsonTxt != null)
        {
            string dataAsJson = JsonTxt.text; // Read the JSON text
            cardData = JsonUtility.FromJson<CardData>(dataAsJson); // Parse the JSON into CardData
        }
        else
        {
            Debug.LogError("JsonTxt is null. Please assign the TextAsset in the Inspector.");
        }
    }

    // Set up the card buttons with concepts and explanations
    void SetupCards()
    {
        cardContentDict = new Dictionary<Button, (string, bool)>(); // Initialize the dictionary to store card contents

        // Create a list to hold all the concepts and explanations
        List<string> allContent = new List<string>();
        foreach (var card in cardData.cards)
        {
            allContent.Add(card.concept); // Add concepts to the list
            allContent.Add(card.explanation); // Add explanations to the list
        }

        // Shuffle the content list to randomize card placement
        allContent = allContent.OrderBy(x => Random.value).ToList();

        // Assign shuffled content to the buttons
        for (int i = 0; i < cardButtons.Count; i++)
        {
            int index = i; // Local copy for use in lambda function
            string content = allContent[i]; // Get the shuffled content
            bool isConcept = cardData.cards.Any(card => card.concept == content); // Check if the content is a concept

            cardContentDict[cardButtons[i]] = (content, isConcept); // Store the content and whether it's a concept
            cardButtons[i].onClick.AddListener(() => OnCardClicked(cardButtons[index])); // Add click listener for the card

            TextMeshProUGUI buttonText = cardButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = content; // Set the button's text

            // Adjust text properties based on the language
            if (LanguageController.instance.currentLanguage == Language.English)
            {
                buttonText.font = LanguageController.instance.otherFont;
                buttonText.isRightToLeftText = false;
            }
            else if (LanguageController.instance.currentLanguage == Language.Arabic)
            {
                buttonText.font = LanguageController.instance.arabicFont;
                buttonText.isRightToLeftText = true;
            }
            else if (LanguageController.instance.currentLanguage == Language.Hebrew)
            {
                buttonText.isRightToLeftText = true;
                buttonText.font = LanguageController.instance.otherFont;
            }

            cardButtons[i].transform.GetChild(0).gameObject.SetActive(false); // Initially hide the text
        }
    }

    // Handle card click events
    void OnCardClicked(Button clickedCard)
    {
        // Return if cards can't be selected or if the card is already selected
        if (!canSelect || clickedCard == firstCard || clickedCard.transform.GetChild(0).gameObject.activeSelf)
            return;

        // Show the card's text (reveal the card)
        clickedCard.GetComponent<Animator>().SetBool("show", true);

        // If no card has been selected yet, store the current card as the firstCard
        if (firstCard == null)
        {
            firstCard = clickedCard;
        }
        else
        {
            // Store the current card as the secondCard and check for a match
            secondCard = clickedCard;
            StartCoroutine(CheckMatch());
        }
    }

    // Coroutine to check if two selected cards are a match
    IEnumerator CheckMatch()
    {
        canSelect = false; // Prevent further card selections

        // Wait for a moment to let the player see the second card
        yield return new WaitForSeconds(1f);

        // Get content of the first and second cards
        var firstCardContent = cardContentDict[firstCard];
        var secondCardContent = cardContentDict[secondCard];

        // Check if the selected cards form a correct match (concept and explanation)
        if (firstCardContent.Item2 != secondCardContent.Item2 &&
            cardData.cards.Any(card =>
                (card.concept == firstCardContent.Item1 && card.explanation == secondCardContent.Item1) ||
                (card.concept == secondCardContent.Item1 && card.explanation == firstCardContent.Item1)))
        {
            // If the cards match, disable them and play a correct sound
            firstCard.interactable = false;
            secondCard.interactable = false;
            SoundManager.instance.PlaySound("correct choice");
        }
        else
        {
            // If the cards don't match, hide the text again and play an error sound
            firstCard.GetComponent<Animator>().SetBool("show", false);
            secondCard.GetComponent<Animator>().SetBool("show", false);
            SoundManager.instance.PlaySound("error");
        }

        // Reset the first and second card selections
        firstCard = null;
        secondCard = null;
        canSelect = true; // Allow selecting cards again

        CheckGameEnd(); // Check if the game has ended
    }

    // Check if all cards have been matched, ending the game
    void CheckGameEnd()
    {
        // If all card buttons are disabled, the game is over
        GameEnded = cardButtons.All(button => !button.interactable);
        if (GameEnded)
        {
            GetComponent<ScreenItem>().OnScreenEnded(); // Trigger the end of the screen
        }
    }
}
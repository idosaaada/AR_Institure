using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMaterials : MonoBehaviour
{
    // Arrays of buttons representing printers and materials
    public Button[] printerButtons;
    public Button[] materialButtons;

    // Variables to track the selected printer and material (-1 means nothing selected)
    private int selectedPrinter = -1;
    private int selectedMaterial = -1;

    // Dictionary defining the correct printer-material matches
    private Dictionary<int, int> correctMatches = new Dictionary<int, int>()
    {
        { 0, 0 }, // Material1 matches Printer1
        { 1, 1 }, // Material2 matches Printer2
        { 2, 2 }, // Material3 matches Printer3
        { 3, 3 }  // Material4 matches Printer4
    };

    // Start is called before the first frame update
    void Start()
    {
        // Set up listeners for printer buttons
        for (int i = 0; i < printerButtons.Length; i++)
        {
            int index = i; // Local copy of index to avoid closure issues
            printerButtons[i].onClick.AddListener(() => OnPrinterSelected(index));
        }

        // Set up listeners for material buttons
        for (int i = 0; i < materialButtons.Length; i++)
        {
            int index = i; // Local copy of index to avoid closure issues
            materialButtons[i].onClick.AddListener(() => OnMaterialSelected(index));
        }
    }

    // Method called when a printer is selected
    void OnPrinterSelected(int printerIndex)
    {
        selectedPrinter = printerIndex; // Store selected printer index
        CheckMatch(); // Check if there is a match
    }

    // Method called when a material is selected
    void OnMaterialSelected(int materialIndex)
    {
        selectedMaterial = materialIndex; // Store selected material index
        CheckMatch(); // Check if there is a match
    }

    // Check if the selected printer and material are a correct match
    void CheckMatch()
    {
        // Only check if both printer and material have been selected
        if (selectedPrinter != -1 && selectedMaterial != -1)
        {
            // Check if the selected printer and material are a correct match
            if (correctMatches[selectedPrinter] == selectedMaterial)
            {
                Debug.Log("Correct Match!");
                // Disable the matched buttons so they can't be selected again
                printerButtons[selectedPrinter].interactable = false;
                materialButtons[selectedMaterial].interactable = false;
                SoundManager.instance.PlaySound("Victory1");
            }
            else
            {
                Debug.Log("Wrong Match!");
                SoundManager.instance.PlaySound("Error");
            }

            // Reset selections after the match check
            selectedPrinter = -1;
            selectedMaterial = -1;

            // Check if the game should end (all matches found)
            bool endGame = true;
            foreach (var item in printerButtons)
            {
                if (item.interactable) // If any button is still interactable, game isn't over
                {
                    endGame = false;
                }
            }

            // If all matches are found, end the game
            if (endGame)
            {
                GetComponent<ScreenItem>().OnScreenEnded();
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

// Enum to define the available languages
public enum Language
{
    English,
    Hebrew,
    Arabic
}

public class LanguageController : MonoBehaviour
{
    public static LanguageController instance;
    public TMP_FontAsset arabicFont;
    public TMP_FontAsset otherFont;

    // Dictionary to store the key-value pairs for translations
    private Dictionary<string, string> translations;

    // A set to track languages that require Right-To-Left (RTL) text direction
    private HashSet<Language> rtlLanguages = new HashSet<Language> { Language.Hebrew, Language.Arabic };

    // The currently selected language
    public Language currentLanguage = Language.Hebrew;

    // TranslationData will store translation items loaded from the JSON file
    public TranslationData translationData;

    public TextMeshProUGUI[] popUptxts;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnEnable()
    {
        SetHebrewLanguage();  // Set the default language to Hebrew
    }

    // Methods to set specific languages and trigger translation updates
    public void SetEnglishLanguage()
    {
        currentLanguage = Language.English;
        JsonTxt = English;
        LoadTranslations();
        TranslateAllText();
    }

    public void SetHebrewLanguage()
    {
        currentLanguage = Language.Hebrew;
        JsonTxt = Hebrew;
        LoadTranslations();
        TranslateAllText();
    }

    public void SetArabicLanguage()
    {
        currentLanguage = Language.Arabic;
        JsonTxt = Arabic;
        LoadTranslations();
        TranslateAllText();
    }

    // TextAssets that store the translation data in JSON format for different languages
    public TextAsset Hebrew, English, Arabic;

    // TextAsset to hold the currently selected language file
    public TextAsset JsonTxt;

    // Method to load translations from the selected JSON file
    void LoadTranslations()
    {
        // Access the text from the TextAsset directly, no need to use File.ReadAllText()
        string jsonContent = JsonTxt.text;

        // Parse the JSON content
        translationData = JsonUtility.FromJson<TranslationData>(jsonContent);

        // If parsing is successful, store the translations in a dictionary
        if (translationData != null && translationData.translations != null)
        {
            translations = translationData.translations.ToDictionary(t => t.key, t => t.value);
            Debug.Log("Translations loaded successfully!");
            Debug.Log("Keys: " + string.Join(", ", translations.Keys));
        }
        else
        {
            Debug.LogError("Failed to parse the JSON file or translations are null.");
        }
    }

    // Method to translate all TextMeshProUGUI components in the scene
    public void TranslateAllText()
    {
        if (translations == null)
        {
            Debug.LogError("Translations not loaded!");
            return;
        }

        foreach (var t in popUptxts)
        {
            string originalText = t.text;

            // Check if the translation exists and apply it
            if (translations.ContainsKey(originalText))
            {
                string translatedText = translations[originalText];
                t.text = translatedText;
            }

            if(currentLanguage == Language.Arabic)
            {
                t.font = arabicFont;
            }
            else
            {
                t.font = otherFont;
            }

            // Enable RTL for languages that require it
            t.isRightToLeftText = rtlLanguages.Contains(currentLanguage);
        }

        // Find all TextMeshProUGUI components in the scene
        TextMeshProUGUI[] textMeshPros = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

        foreach (var textMeshPro in textMeshPros)
        {
            string originalText = textMeshPro.gameObject.name;

            // Check if the translation exists and apply it
            if (translations.ContainsKey(originalText))
            {
                string translatedText = translations[originalText];
                textMeshPro.text = translatedText;

                // Enable RTL for languages that require it
                textMeshPro.isRightToLeftText = rtlLanguages.Contains(currentLanguage);
            }

            if(currentLanguage == Language.Arabic)
            {
                textMeshPro.font = arabicFont;
            }
            else
            {
                textMeshPro.font = otherFont;
            }
        }
    }

    // Classes to represent translation data
    [System.Serializable]
    public class TranslationItem
    {
        public string key;   // Original text key (e.g., the object's name)
        public string value; // Translated text value
    }

    [System.Serializable]
    public class TranslationData
    {
        public List<TranslationItem> translations;  // List of all translations
    }
}

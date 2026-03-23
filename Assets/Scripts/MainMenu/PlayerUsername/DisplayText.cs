using UnityEngine;
using TMPro;           // For TextMeshPro text elements

public class DisplayText : MonoBehaviour
{
    // This text will show the saved username
    public TextMeshProUGUI obj_text;

    // This input field is where the player types their name
    public TMP_InputField display;

    //  Show a welcome message
    public TextMeshProUGUI WelcomeText;

    // store the current username so it can be reused in Start() and Create()
    private string user_name;

    // Start is called when the game begins
    void Start()
    {
        // Load the saved name when game starts
        user_name = PlayerPrefs.GetString("user_name", "Player");
        obj_text.text = user_name;

        // Update welcome message with the loaded name
        WelcomeText.text = $"Welcome back, {user_name}!";
    }

    // This function runs when player clicks a button (like "Save" or "Submit")
    public void Create()
    {
        // Take what player typed and put it in the text that shows on screen
        user_name = display.text;
        obj_text.text = user_name;

        // Save the name so we can load it next time
        PlayerPrefs.SetString("user_name", user_name);

        // Actually write the data to disk (save it permanently)
        PlayerPrefs.Save();
        Debug.Log("Username saved: " + obj_text.text); // Print to console for confirmation

        // Update welcome message with the new name
        WelcomeText.text = $"Welcome back, {user_name}!";
    }
}
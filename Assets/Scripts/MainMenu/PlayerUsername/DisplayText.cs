using UnityEngine;
using TMPro;           // For TextMeshPro text elements

public class DisplayText : MonoBehaviour
{
    // This text will show the saved username
    public TextMeshProUGUI obj_text;

    // This input field is where the player types their name
    public TMP_InputField display;

    // Start is called when the game begins
    void Start()
    {
        // Load the saved name when game starts
        // PlayerPrefs is like a small storage box that keeps data even after game closes
        // GetString tries to find a saved name. If none exists, it returns empty string
        obj_text.text = PlayerPrefs.GetString("user_name", "Player");
    }

    // This function runs when player clicks a button (like "Save" or "Submit")
    public void Create()
    {
        // Take what player typed and put it in the text that shows on screen
        // display.text = what player typed in the input field
        // obj_text.text = the text that shows on screen
        obj_text.text = display.text; 

        // STEP 3: Save the name so we can load it next time
        // "user_name" is like a label for our saved data
        // We save whatever is now in obj_text.text
        PlayerPrefs.SetString("user_name", obj_text.text);

        // Actually write the data to disk (save it permanently)
        PlayerPrefs.Save();
        Debug.Log("Username saved: " + obj_text.text); // Print to console for confirmation
    }
}
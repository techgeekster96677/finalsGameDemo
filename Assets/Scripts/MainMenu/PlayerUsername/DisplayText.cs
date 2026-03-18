using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using TMPro;           // For TextMeshPro text elements
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DisplayText : MonoBehaviour
{
    // This text will show the saved username
    public TextMeshProUGUI obj_text;

    // This is where the player types their name (input field)
    public TextMeshProUGUI display;

    // Start is called when the game begins
    void Start()
    {
        // STEP 1: Load the saved name when game starts
        // PlayerPrefs is like a small storage box that keeps data even after game closes
        // GetString tries to find a saved name. If none exists, it returns empty string
        obj_text.text = PlayerPrefs.GetString("user_name");

        // Example: If player saved "John" last time, obj_text will show "John"
    }

    // This function runs when player clicks a button (like "Save" or "Submit")
    public void Create()
    {
        // STEP 2: Take what player typed and put it in obj_text
        // display.text = what player typed in input field
        // obj_text.text = the text that shows on screen
        obj_text.text = display.text;

        // STEP 3: Save the name so we can load it next time
        // "user_name" is like a label for our saved data
        // We save whatever is now in obj_text.text
        PlayerPrefs.SetString("user_name", obj_text.text);

        // STEP 4: Actually write the data to disk (save it permanently)
        PlayerPrefs.Save();

        // Now if you close and reopen the game, the name will still be there!
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro; // For TextMeshPro text elements

public class DialogueManager : MonoBehaviour
{
    // Singleton pattern to ensure only one instance of DialogueManager exists
    public static DialogueManager Instance;

    // UI elements for the dialogue box
    public Image characterIcon; 
    public TextMeshProUGUI characterName;
    public string playerName = "Player"; // Default player name, can be set from elsewhere
    public TextMeshProUGUI dialogueArea;

    // Queue to hold the lines of dialogue
    private Queue<DialogueLine> lines;

    // Flag to check if dialogue is currently active
    public bool isDialogueActive = false;

    // Speed at which the text is typed out in the dialogue box
    public float typingSpeed = 0.2f;

    // Animator to control the show/hide animations of the dialogue box
    public Animator animator;


    // To initialize the singleton instance and set up the dialogue lines queue
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();

        // Load the player's name from PlayerPrefs, or use the default if not set
        playerName = PlayerPrefs.GetString("user_name", "Player");

        Debug.Log("Loaded playerName = " + playerName);
    }


    // Method to start a dialogue sequence
    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true; // Set the dialogue active flag to true

        animator.Play("show"); // Play the show animation for the dialogue box

        lines.Clear(); // Clear any existing lines in the queue

        // foreach loop to enqueue each line of dialogue from the provided Dialogue object
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine(); // Display the first line of dialogue
    }


    // Method to display the next line of dialogue
    public void DisplayNextDialogueLine()
    {
        // Check if there are no more lines to display
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Dequeue the next line of dialogue from the queue
        DialogueLine currentLine = lines.Dequeue();

        // If the character is the player, set the name to the playerName variable
        characterIcon.sprite = currentLine.character.icon;

        // If the character is the player, set the name to the playerName variable
        // otherwise use the character's name
        Debug.Log("Character in dialogue = " + currentLine.character.name);
        Debug.Log("Player name = " + playerName);

        if (currentLine.character.name == "Player")
        {
            characterName.text = playerName;
        }
        else
        {
            characterName.text = currentLine.character.name;
        }

        // coroutine to type out the dialogue line letter by letter in the dialogue area
        StopAllCoroutines();

        // startcoroutine to type out the dialogue line letter by letter in the dialogue area
        StartCoroutine(TypeSentence(currentLine));
    }


    // Coroutine to delay the display of the first line of dialogue
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Coroutine to delay the display of the first line of dialogue
    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("hide");
    }
}

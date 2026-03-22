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

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
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
        characterName.text = currentLine.character.name;

        // coroutine to type out the dialogue line letter by letter in the dialogue area
        StopAllCoroutines();

        // startcoroutine to type out the dialogue line letter by letter in the dialogue area
        StartCoroutine(TypeSentence(currentLine));
    }

    /// <summary>
    /// Animates the display of a dialogue line by revealing its text one character at a time in the dialogue area.
    /// </summary>
    /// <remarks>Use this coroutine to create a typewriter effect for dialogue text. The speed of the
    /// animation is determined by the typing speed setting.</remarks>
    /// <param>The dialogue line to display, containing the text to be animated.</param>
    /// <returns>An enumerator that performs the character-by-character text animation when iterated.</returns>
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    /// <summary>
    /// Ends the current dialogue session and hides the dialogue interface.
    /// </summary>
    /// <remarks>Call this method to terminate an active dialogue. After calling, the dialogue UI will be
    /// hidden and no further dialogue input will be processed until a new session is started.</remarks>
    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("hide");
    }
}

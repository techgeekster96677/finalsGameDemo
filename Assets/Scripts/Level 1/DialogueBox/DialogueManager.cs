using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages dialogue display with typewriter effect and character portraits.
/// Implements singleton pattern for global access across scenes.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;

    public Animator animator;

    /// <summary>
    /// Singleton initialization. Ensures only one instance exists.
    /// Initializes the queue for dialogue lines.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    /// <summary>
    /// Starts a new dialogue sequence.
    /// 
    /// BEHAVIOR:
    /// - Sets isDialogueActive to true
    /// - Plays "show" animation to display dialogue UI
    /// - Clears and refills the queue with dialogue lines
    /// - Begins displaying the first line
    /// </summary>
    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;

        animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    /// <summary>
    /// Displays the next line in the dialogue queue.
    /// If no lines remain, ends the dialogue.
    /// 
    /// BEHAVIOR:
    /// - Updates character icon and name
    /// - Stops any ongoing typewriter coroutine
    /// - Starts typing effect for the current line
    /// </summary>
    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    /// <summary>
    /// Coroutine that displays text one character at a time.
    /// 
    /// BEHAVIOR:
    /// - Clears existing text
    /// - Adds each character sequentially with delay between letters
    /// - Delay duration is controlled by typingSpeed
    /// </summary>
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
    /// Ends the current dialogue sequence.
    /// 
    /// BEHAVIOR:
    /// - Sets isDialogueActive to false
    /// - Plays "hide" animation to dismiss dialogue UI
    /// </summary>
    void EndDialogue()
    {
        isDialogueActive = false;
        animator.Play("hide");
    }
}
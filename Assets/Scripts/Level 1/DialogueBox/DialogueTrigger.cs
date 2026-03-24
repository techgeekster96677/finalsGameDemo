using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure for a character appearing in dialogue.
/// Contains character name and portrait icon.
/// </summary>
[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

/// <summary>
/// Data structure for a single line of dialogue.
/// Associates a character with their spoken line.
/// </summary>
[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

/// <summary>
/// Data structure for a complete dialogue sequence.
/// Contains a list of dialogue lines to display in order.
/// </summary>
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

/// <summary>
/// Triggers dialogue when the player enters the trigger collider.
/// Can also be called manually from other scripts or UI events.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool hasTriggered = false;

    /// <summary>
    /// Manually triggers the dialogue from external scripts or UI buttons.
    /// </summary>
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    /// <summary>
    /// Triggers dialogue when the player enters the trigger collider.
    /// 
    /// BEHAVIOR:
    /// - Checks if the entering object has the "Player" tag
    /// - Only triggers once (hasTriggered prevents multiple triggers)
    /// - Logs debug message when any object enters the trigger area
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something entered trigger");

        if (collision.tag == "Player" && !hasTriggered)
        {
            hasTriggered = true;
            TriggerDialogue();
        }
    }
}
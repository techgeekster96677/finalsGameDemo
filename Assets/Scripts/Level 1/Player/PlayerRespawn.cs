using UnityEngine;

// Handles player respawn system using checkpoints
public class PlayerRespawn : MonoBehaviour
{
    // Stores the latest checkpoint the player has reached
    private Transform currentCheckpoint;

    // Reference to Health script for resetting player state
    private Health playerHealth;

    // Reference to UIManager for showing game over screen
    private UIManager uiManager;

    private void Awake()
    {
        // Cache required components
        playerHealth = GetComponent<Health>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    // Respawns player at the last checkpoint
    public void RespawnToCheckpoint()
    {
        // If no checkpoint has been reached yet
        if (currentCheckpoint == null)
        {
            // Show game over or restart UI
            uiManager.ShowGameOverUI();
            return;
        }
        else
        {
            // Play checkpoint sound
            SoundManager.Instance.PlaySound2D("Checkpoint Appear");

            // Move player to checkpoint position
            transform.position = currentCheckpoint.position;

            // Reset player health and state
            playerHealth.Respawn();
        }
    }

    // Detects when player reaches a checkpoint
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            // Save this checkpoint
            currentCheckpoint = collision.transform;

            // Play checkpoint activation sound
            SoundManager.Instance.PlaySound2D("Checkpoint Appear");

            // Disable collider so it doesn't trigger again
            collision.GetComponent<Collider2D>().enabled = false;

            // Trigger checkpoint animation if available
            collision.GetComponent<Animator>()?.SetTrigger("appear");
        }
    }
}
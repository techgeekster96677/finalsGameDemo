using UnityEngine;

// Handles level transition when the player enters the portal
public class Portal : MonoBehaviour
{
    // Reference to UIManager for showing next level screen
    private UIManager uiManager;

    public void Awake()
    {
        // Find UIManager in the scene (used to control UI display)
        uiManager = FindFirstObjectByType<UIManager>();
    }

    // Triggered when another collider enters the portal
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Pause the game by stopping time
            Time.timeScale = 0;

            // Show next level or completion UI
            uiManager.ShowNextLevelUI();

            // Play portal sound effect
            SoundManager.Instance.PlaySound2D("Portal Enter");
        }
    }
}
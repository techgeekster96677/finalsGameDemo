using UnityEngine;
using UnityEngine.UI;

// Updates the UI health bar based on the player's current health
public class Healthbar : MonoBehaviour
{
    // Reference to the player's Health script
    [SerializeField] private Health playerHealth;

    // Background (full health bar)
    [SerializeField] private Image totalHealthBar;

    // Foreground (current health that shrinks when damaged)
    [SerializeField] private Image currentHealthBar;

    public void Start()
    {
        /*
        Initializes the total health bar.

        fillAmount ranges from 0 to 1:
        - 1 = full bar
        - 0 = empty

        Dividing by 10 assumes the player's max health is 10.
        */
        totalHealthBar.fillAmount = playerHealth.currentHealth / 10;
    }

    public void Update()
    {
        /*
        Continuously updates the current health bar.

        As the player's health decreases, the fillAmount decreases,
        visually shrinking the bar from right to left.
        */
        currentHealthBar.fillAmount = playerHealth.currentHealth / 10;

    }
}
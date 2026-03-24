using UnityEngine;

/// <summary>
/// Health pickup item that restores player health when collected.
/// Disables itself after collection to prevent reuse.
/// </summary>
public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthAmount;

    /// <summary>
    /// Triggers when the player enters the collectible's trigger collider.
    /// 
    /// BEHAVIOR:
    /// - Checks if the entering object has the "Player" tag
    /// - Heals the player by the specified healthAmount
    /// - Plays a pickup sound effect
    /// - Disables the collectible GameObject (prevents re-collection)
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().Heal(healthAmount);
            SoundManager.Instance.PlaySound2D("Health Pickup");
            gameObject.SetActive(false);
        }
    }

}
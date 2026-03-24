using System.Collections;
using UnityEngine;

/// <summary>
/// Manages health, damage, death, invincibility frames, and respawn functionality.
/// Supports both player and enemy entities with configurable death behavior.
/// </summary>
/// 
// Handles health, damage, invincibility (iFrames), death, and respawn
public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float startingHealth;

    // Current health is read-only outside this class
    public float currentHealth { get; private set; }

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool dead;
    private bool isInvincible = false;

    [Header("iFrames")]
    // Duration of temporary invincibility after taking damage
    [SerializeField] private float invincibilityDuration;

    [Header("Death Settings")]
    [SerializeField] private float deathAnimationLength = 1f;
    [SerializeField] private bool destroyEnemyOnDeath = true;
    [SerializeField] private float destroyDelay = 2f;

    // Y position threshold for falling death (for player)
    [SerializeField] private float fallDeathY;

    [Header("Components")]
    // Components to disable when dead (movement, attack, etc.)
    public Behaviour[] components;

    // Determines if this object is the player
    private bool isPlayer;

    private void Awake()
    {
        // Initialize health
        currentHealth = startingHealth;

        // Cache components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if this object has Player script
        isPlayer = GetComponent<Player>() != null;
    }

    private void Update()
    {
        // If player falls below a certain Y position, trigger death
        if (isPlayer && !dead && transform.position.y < fallDeathY)
        {
            StartCoroutine(Die());
        }
    }

    /// <summary>
    /// Applies damage to the entity.
    /// 
    /// BEHAVIOR:
    /// - Prevents damage if already dead or invincible
    /// - Reduces current health and clamps between 0 and startingHealth
    /// - Triggers hurt animation and plays sound if health remains
    /// - Triggers death sequence if health reaches 0
    /// </summary>
    // Applies damage and handles health reduction
    public void TakeDamage(float damage)
    {
        // Ignore damage if already dead or currently invincible
        if (dead || isInvincible)
            return;

        // Reduce health but keep within 0 to startingHealth
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Play hurt animation and sound
            if (animator != null)
            {
                animator.SetTrigger("hurt");
                SoundManager.Instance.PlaySound2D(isPlayer ? "Player Hurt" : "Enemy Hurt");
            }

            // Activate temporary invincibility
            StartCoroutine(Invincibility());
        }
        else
        {
            // Trigger death if health reaches zero
            if (!dead)
            {
                StartCoroutine(Die());
            }
        }
    }
    /// <summary>
    /// Handles death sequence including animation, sound, and component disabling.
    /// 
    /// BEHAVIOR:
    /// - Triggers death animation and sound
    /// - Disables all components in the components array
    /// - Waits for death animation to complete
    /// - Destroys enemy GameObject if configured (players are not destroyed)
    /// </summary>
    // Handles death behavior using coroutine for timing
    private IEnumerator Die()
    {
        dead = true;

        // Play death animation and sound
        if (animator != null)
        {
            animator.SetTrigger("die");
            SoundManager.Instance.PlaySound2D(isPlayer ? "Player Death" : "Enemy Death");
            Debug.Log($"{gameObject.name}: Death animation triggered");
        }

        // Disable movement, attack, and other behaviors
        foreach (Behaviour component in components)
            component.enabled = false;

        // Wait for animation to finish
        yield return new WaitForSeconds(deathAnimationLength);

        // Destroy only enemies (player is handled differently)
        if (!isPlayer && destroyEnemyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    /// <summary>
    /// Restores health by the specified amount.
    /// Health is clamped between 0 and startingHealth.
    /// </summary>
    // Restores health
    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
    }

    // Resets player after death
    public void Respawn()
    {
        dead = false;

        // Restore full health
        Heal(startingHealth);

        // Reset animation state
        animator.ResetTrigger("die");
        animator.Play("player_idle");

        // Give temporary invincibility after respawn
        StartCoroutine(Invincibility());

        // Re-enable disabled components
        foreach (Behaviour component in components)
            component.enabled = true;
    }

    /// <summary>
    /// Coroutine that provides temporary invincibility after taking damage.
    /// 
    /// BEHAVIOR:
    /// - Ignores collisions between layers 6 and 7
    /// - Provides visual feedback by blinking the sprite renderer
    /// - Restores collision after invincibility duration
    /// </summary>
    // Handles temporary invincibility with blinking effect
    private IEnumerator Invincibility()
    {
        isInvincible = true;

        // Ignore collisions between specific layers (e.g., player and enemy)
        Physics2D.IgnoreLayerCollision(6, 7, true);

        if (spriteRenderer != null)
        {
            float blinkInterval = 0.1f;
            float elapsedTime = 0f;

            // Toggle sprite visibility to create blinking effect
            while (elapsedTime < invincibilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsedTime += blinkInterval;
            }

            // Ensure sprite is visible after blinking ends
            spriteRenderer.enabled = true;
        }
        else
        {
            // If no sprite renderer, just wait duration
            yield return new WaitForSeconds(invincibilityDuration);
        }

        // Re-enable collisions and allow damage again
        Physics2D.IgnoreLayerCollision(6, 7, false);
        isInvincible = false;
    }
    /// <summary>
    /// Resets invincibility and death state when the object is re-enabled.
    /// Ensures collision layers are properly restored.
    /// </summary>
    private void OnEnable()
    {
        // Reset states when object is enabled
        isInvincible = false;
        dead = false;

        Physics2D.IgnoreLayerCollision(6, 7, false);

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        // Ensure collisions are restored when object is disabled
        Physics2D.IgnoreLayerCollision(6, 7, false);
    }
}
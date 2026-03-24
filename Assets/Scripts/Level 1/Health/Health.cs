using System.Collections;
using UnityEngine;

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
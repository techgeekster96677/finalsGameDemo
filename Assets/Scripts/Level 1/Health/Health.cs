using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float startingHealth;
    public float currentHealth { get; private set; }
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool dead;
    private bool isInvincible = false;

    [Header("iFrames")]
    [SerializeField] private float invincibilityDuration;

    [Header("Death Settings")]
    [SerializeField] private float deathAnimationLength = 1f;
    [SerializeField] private bool destroyEnemyOnDeath = true;
    [SerializeField] private float destroyDelay = 2f;

    // Reference to check if this is the player
    private bool isPlayer;

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if this is the player
        isPlayer = GetComponent<Player>() != null;
    }

    public void TakeDamage(float damage)
    {
        // Don't take damage if already dead or invincible
        if (dead || isInvincible)
            return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            //hurt
            if (animator != null)
                animator.SetTrigger("hurt");
            StartCoroutine(Invincibility());
        }
        else
        {
            if (!dead)
            {
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die()
    {
        dead = true;

        // Stop any movement immediately
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            // Fix: Use bodyType instead of isKinematic
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Disable collider so things can pass through
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("die");
            Debug.Log($"{gameObject.name}: Death animation triggered");
        }

        // Disable components based on what this is
        Player player = GetComponent<Player>();
        if (player != null)
        {
            // This is the player - disable player controls but keep camera following?
            player.enabled = false;
            Debug.Log("Player died - game over!");
            // You might want to show game over screen here
        }

        // Enemy specific (only disable if this is an enemy)
        EnemyPatrol enemyPatrol = GetComponentInParent<EnemyPatrol>();
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
        }

        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.enabled = false;
        }

        // Wait for death animation to play
        yield return new WaitForSeconds(deathAnimationLength);

        // Only destroy if this is an enemy and destroyEnemyOnDeath is true
        if (!isPlayer && destroyEnemyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
        // For player, we don't destroy - we might reload scene or show game over
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        Physics2D.IgnoreLayerCollision(6, 7, true);

        // Visual feedback with blinking
        if (spriteRenderer != null)
        {
            float blinkInterval = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < invincibilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
                elapsedTime += blinkInterval;
            }

            // Make sure sprite is visible at the end
            spriteRenderer.enabled = true;
        }
        else
        {
            // If no sprite renderer, just wait the duration
            yield return new WaitForSeconds(invincibilityDuration);
        }

        Physics2D.IgnoreLayerCollision(6, 7, false);
        isInvincible = false;
    }

    private void OnEnable()
    {
        isInvincible = false;
        dead = false;
        Physics2D.IgnoreLayerCollision(6, 7, false);

        // Make sure sprite is visible when enabled
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        Physics2D.IgnoreLayerCollision(6, 7, false);
    }
}
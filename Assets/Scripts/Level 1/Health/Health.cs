using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float startingHealth;
    public float currentHealth { get; private set; }
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Store this as a class variable
    private bool dead;
    private bool isInvincible = false;

    [Header("iFrames")]
    [SerializeField] private float invincibilityDuration;

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Store in class variable
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
            animator.SetTrigger("hurt");
            StartCoroutine(Invincibility()); // Now this works without parameters
        }
        else
        {
            if (!dead)
            {
                //die
                animator.SetTrigger("die");

                //player
                if(GetComponent<Player>() != null)
                    GetComponent<Player>().enabled = false;

                //enemy
                if(GetComponentInParent<EnemyPatrol>() != null)
                    GetComponentInParent<EnemyPatrol>().enabled = false;

                if(GetComponent<Enemy>() != null)
                    GetComponent<Enemy>().enabled = false;

                dead = true;
            }
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, startingHealth);
    }

    //Testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    private IEnumerator Invincibility() // Removed parameter
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
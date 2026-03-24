using UnityEngine;

// Handles boss behavior: attacking, taking damage, and interacting with the player
public class Boss : MonoBehaviour
{
    // Boss health
    public int health = 5;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        // Animator component
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Stop behavior if boss is dead
        if (isDead) return;

        // Keep boss in attacking state while alive
        anim.SetBool("isAttacking", true);
    }

    // Called when boss takes damage
    public void TakeDamage(int damage)
    {
        Debug.Log("Boss HIT!");

        // Reduce health
        health -= damage;

        // Play hurt animation
        anim.SetTrigger("Hurt");

        // Check if boss is dead
        if (health <= 0)
        {
            isDead = true;

            // Trigger death animation
            anim.SetBool("isDead", true);

            // Destroy after delay to allow animation to play
            Destroy(gameObject, 2f);
        }
    }

    // Detect collision with player and apply damage
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Boss hit player!");

            // Get player script and deal damage if available
            PlayerWarrior player = col.GetComponent<PlayerWarrior>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
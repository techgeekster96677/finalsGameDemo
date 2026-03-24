using UnityEngine;

// Controls the behavior of the Bat enemy including flying patrol movement,
// animation handling, health management, and interaction with the player.
public class Bat : MonoBehaviour
{
    // Movement speed of the bat
    public float speed = 2f;

    // Patrol points assigned in the Unity Inspector
    public Transform pointA;
    public Transform pointB;

    // Determines direction of movement (true = moving toward pointB)
    private bool goingToB = true;

    // Reference to Animator component for controlling animations
    private Animator anim;

    // Health value of the bat
    public int health = 2;

    // Flag to prevent actions after death
    private bool isDead = false;

    // Called once when the object is initialized
    void Start()
    {
        // Get the Animator component attached to this object
        anim = GetComponent<Animator>();

        // Ensure the bat starts aligned vertically with pointA
        transform.position = new Vector2(transform.position.x, pointA.position.y);
    }

    // Called once per frame
    void Update()
    {
        // Continuously execute patrol behavior
        Patrol();
    }

    // Handles flying movement between two patrol points
    void Patrol()
    {
        // Stop movement if the bat is dead
        if (isDead) return;

        // Trigger flying animation
        anim.SetBool("isFlying", true);

        // Determine the current target point based on direction
        Transform target = goingToB ? pointB : pointA;

        // Move the bat toward the target point
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Calculate distance to the target point
        float distance = Vector2.Distance(transform.position, target.position);

        // If close enough to the target, switch direction
        if (distance < 0.2f)
        {
            goingToB = !goingToB;

            // Flip the sprite to face the direction of movement
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Reduces health when the bat is hit by the player
    public void TakeDamage(int damage)
    {
        // Do nothing if already dead
        if (isDead) return;

        // Subtract damage from health
        health -= damage;

        // Log current health for debugging
        Debug.Log("Bat HP: " + health);

        // Play hurt animation and sound effect
        anim.SetTrigger("Hurt");
        SoundManager.Instance.PlaySound2D("Enemy Slash");

        // Check if health has reached zero or below
        if (health <= 0)
        {
            // Mark as dead to stop further actions
            isDead = true;

            // Trigger death animation
            anim.SetBool("isDead", true);

            // Destroy the bat after a short delay
            Destroy(gameObject, 0.7f);
        }
    }

    // Detects collision with the player and applies damage
    void OnTriggerEnter2D(Collider2D col)
    {
        // Log collision for debugging purposes
        Debug.Log("Bat touched: " + col.name);

        // Check if the collided object is the player
        if (col.CompareTag("Player"))
        {
            // Get the PlayerWarrior component
            PlayerWarrior player = col.GetComponent<PlayerWarrior>();

            // If the player component exists, apply damage
            if (player != null)
            {
                player.TakeDamage(1);

                // Play attack sound effect
                SoundManager.Instance.PlaySound2D("Enemy Slash");
            }
        }
    }
}
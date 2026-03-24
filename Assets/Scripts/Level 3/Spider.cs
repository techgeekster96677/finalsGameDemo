using UnityEngine;

// Controls the behavior of the Spider enemy including movement, animation,
// health management, and interaction with the player.
public class Spider : MonoBehaviour
{
    // Movement speed of the spider
    public float speed = 2f;

    // Patrol points assigned in the Unity Inspector
    public Transform pointA;
    public Transform pointB;

    // Current target point the spider is moving toward
    private Transform target;

    // Reference to Animator component for controlling animations
    private Animator anim;

    // Health value of the spider
    public int health = 2;

    // Flag to prevent actions after death
    private bool isDead = false;

    // Called once when the object is initialized
    void Start()
    {
        // Set initial patrol direction toward pointB
        target = pointB;

        // Get the Animator component attached to this object
        anim = GetComponent<Animator>();
    }

    // Called once per frame
    void Update()
    {
        // Continuously execute patrol behavior
        Patrol();
    }

    // Handles movement between two patrol points
    void Patrol()
    {
        // Stop movement if the spider is dead
        if (isDead) return;

        // Trigger walking animation
        anim.SetBool("isWalking", true);

        // Move the spider toward the current target point
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Check if the spider is close enough to the target point
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            // Switch target to the opposite patrol point
            target = (target == pointA) ? pointB : pointA;

            // Flip the sprite to face the direction of movement
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Reduces health when the spider is hit by the player
    public void TakeDamage(int damage)
    {
        // Do nothing if already dead
        if (isDead) return;

        // Subtract damage from health
        health -= damage;

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

            // Destroy the spider after a short delay
            Destroy(gameObject, 0.5f);
        }
    }

    // Detects collision with the player and applies damage
    void OnTriggerEnter2D(Collider2D col)
    {
        // Log collision for debugging purposes
        Debug.Log("Something touched spider: " + col.name);

        // Check if the collided object is the player
        if (col.CompareTag("Player"))
        {
            // Log player hit event
            Debug.Log("PLAYER HIT BY SPIDER!");

            // Play attack sound effect
            SoundManager.Instance.PlaySound2D("Enemy Slash");

            // Access the PlayerWarrior script and apply damage
            PlayerWarrior player = col.GetComponent<PlayerWarrior>();

            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
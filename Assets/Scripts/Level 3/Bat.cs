using UnityEngine;

public class Bat : MonoBehaviour
{
    // Patrol settings
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    // Internal state
    private bool goingToB = true;
    private Animator anim;

    // Health and death handling
    public int health = 2;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        // start at correct height
        transform.position = new Vector2(transform.position.x, pointA.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (isDead) return;

        // Set flying animation
        anim.SetBool("isFlying", true);

        // Determine current target point
        Transform target = goingToB ? pointB : pointA;

        // Move towards the target point
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position, 
            speed * Time.deltaTime
        );

        // Check if we've reached the target point
        float distance = Vector2.Distance(transform.position, target.position);

        // If we're close enough to the target, switch direction
        if (distance < 0.2f)
        {
            goingToB = !goingToB;

            // flip
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // This method can be called by the player's attack script when the bat is hit
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        Debug.Log("Bat HP: " + health);

        // Play hurt animation and sound
        anim.SetTrigger("Hurt");
        SoundManager.Instance.PlaySound2D("Enemy Slash");

        // If health drops to 0 or below, handle death
        if (health <= 0)
        {
            isDead = true;
            anim.SetBool("isDead", true);

            Destroy(gameObject, 0.7f);
        }
    }

    // Handle collision with player to apply damage
    void OnTriggerEnter2D(Collider2D col)
    {
        // Only apply damage if the bat is alive
        Debug.Log("Bat touched: " + col.name);

        // Check if the collider belongs to the player
        if (col.CompareTag("Player"))
        {
            // Try to get the PlayerWarrior component from the collider
            PlayerWarrior player = col.GetComponent<PlayerWarrior>();

            // If the component exists, apply damage to the player
            if (player != null)
            {
                player.TakeDamage(1);
                SoundManager.Instance.PlaySound2D("Enemy Slash");
            }
        }
    }
}
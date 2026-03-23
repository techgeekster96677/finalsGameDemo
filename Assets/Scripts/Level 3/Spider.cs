using UnityEngine;

public class Spider : MonoBehaviour
{
    // Patrol settings
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    // Internal state
    private Transform target;
    private Animator anim;

    // Health and death handling
    public int health = 2;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        target = pointB;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    // Simple patrol logic between pointA and pointB
    void Patrol()
    {
        if (isDead) return;

        anim.SetBool("isWalking", true);

        // Move towards the target point
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Check if we've reached the target point
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            // Switch target
            target = (target == pointA) ? pointB : pointA;

            // Flip the sprite to face the new direction
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // This method can be called by the player's attack script when the spider is hit
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        // Play hurt animation and sound
        anim.SetTrigger("Hurt");
        SoundManager.Instance.PlaySound2D("Enemy Slash");

        if (health <= 0)
        {
            isDead = true;
            anim.SetBool("isDead", true);

            Destroy(gameObject, 0.5f);
        }
    }

    // This method should be called via Animation Event at the moment of attack
    void OnTriggerEnter2D(Collider2D col)
    {
        // Only check for player collision if we're currently attacking
        Debug.Log("Something touched spider: " + col.name);

        // Check if the collider belongs to the player
        if (col.CompareTag("Player"))
        {
            // Apply damage to the player
            Debug.Log("PLAYER HIT BY SPIDER!");
            SoundManager.Instance.PlaySound2D("Enemy Slash");

            col.GetComponent<PlayerWarrior>().TakeDamage(1);
        }
    }
}
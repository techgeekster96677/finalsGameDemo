using UnityEngine;

public class Bat : MonoBehaviour
{
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    private bool goingToB = true;
    private Animator anim;

    public int health = 2;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        // start at correct height
        transform.position = new Vector2(transform.position.x, pointA.position.y);
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (isDead) return;

        anim.SetBool("isFlying", true);

        Transform target = goingToB ? pointB : pointA;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position, // bat can move freely (X + Y)
            speed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance < 0.2f)
        {
            goingToB = !goingToB;

            // flip
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        anim.SetTrigger("Hurt");

        if (health <= 0)
        {
            isDead = true;
            anim.SetBool("isDead", true);

            Destroy(gameObject, 0.7f);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerWarrior>().TakeDamage(1);
        }
    }
}
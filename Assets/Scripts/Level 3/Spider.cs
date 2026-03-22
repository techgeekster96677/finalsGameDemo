using UnityEngine;

public class Spider : MonoBehaviour
{
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    private Transform target;
    private Animator anim;

    public int health = 2;
    private bool isDead = false;

    void Start()
    {
        target = pointB;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (isDead) return;

        anim.SetBool("isWalking", true);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;

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

            Destroy(gameObject, 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Something touched spider: " + col.name);

        if (col.CompareTag("Player"))
        {
            Debug.Log("PLAYER HIT BY SPIDER!");

            col.GetComponent<PlayerWarrior>().TakeDamage(1);
        }
    }
}
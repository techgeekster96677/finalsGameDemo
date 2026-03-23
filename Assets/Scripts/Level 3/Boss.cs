using UnityEngine;

public class Boss : MonoBehaviour
{
    public int health = 5;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        anim.SetBool("isAttacking", true);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Boss HIT!");

        health -= damage;

        anim.SetTrigger("Hurt");

        if (health <= 0)
        {
            anim.SetBool("isDead", true);
            Destroy(gameObject, 2f);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Boss hit player!");

            PlayerWarrior player = col.GetComponent<PlayerWarrior>();

            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
using UnityEngine;

public class PlayerWarrior : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 2f;
    public LayerMask enemyLayer;
    public int attackDamage = 1;

    [Header("Health")]
    public int health = 3;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isDead = false;
    private float move;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        move = Input.GetAxis("Horizontal");

        Move();
        Jump();
        Attack();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        if (move > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else if (move < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    bool canAttack = true;
    public float attackCooldown = 0.5f;

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            Debug.Log("ATTACK!");

            canAttack = false;

            anim.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

            Debug.Log("Enemies hit: " + hitEnemies.Length);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.SendMessage("TakeDamage", 1);
            }

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void HandleAnimation()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(move));
        anim.SetBool("isGrounded", isGrounded);

        // Make sure this parameter exists in Animator!
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        anim.SetTrigger("Hurt");

        Debug.Log("Player HP: " + health);

        if (health <= 0)
        {
            isDead = true;
            anim.SetBool("isDead", true);

            rb.linearVelocity = Vector2.zero;
        }
    }

    // Optional: visualize attack range
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
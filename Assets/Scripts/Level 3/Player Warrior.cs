using TMPro;
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
    public float attackRange = 4f;
    public LayerMask enemyLayer;
    public int attackDamage = 1;

    //[Header("Health")]
    //public int health = 3;
    private Health healthScript;
    public GameObject gameOverScreen;

    // Add attack origin for directional attacks
    [Header("Attack Origin")]
    [SerializeField] private Transform attackOrigin;
    private Vector3 attackOriginInitialLocalPos;

    // Player state components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isDead = false;
    private float move;

    // Footstep sound cooldown
    private float lastFootstepTime = 0f;
    public float footstepCooldown = 0.3f;

    // Gem collection
    private int gemCounter = 0;
    public int totalGems = 0;
    public TextMeshProUGUI counterText;

    // Attack cooldown
    private bool canAttack = true;
    public float attackCooldown = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<Health>();

        // Store the initial local position of attack origin
        if (attackOrigin != null)
        {
            attackOriginInitialLocalPos = attackOrigin.localPosition;
        }

        // Initialize gem counter UI
        if (totalGems <= 0)
        {
            totalGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        }

        counterText.text = $"Gems: {gemCounter}/{totalGems}";
    }

    void Update()
    {
        if (isDead) return;

        move = Input.GetAxis("Horizontal");

        Move();
        Jump();
        Attack();
        HandleAnimation();
        HandleFootstepSounds();
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Flip character based on movement direction
        if (move > 0)
        {
            // Face right
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Update attack origin position when facing right
            if (attackOrigin != null)
            {
                Vector3 newLocalPos = attackOriginInitialLocalPos;
                newLocalPos.x = Mathf.Abs(attackOriginInitialLocalPos.x);
                attackOrigin.localPosition = newLocalPos;
            }
        }
        else if (move < 0)
        {
            // Face left
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Update attack origin position when facing left
            if (attackOrigin != null)
            {
                Vector3 newLocalPos = attackOriginInitialLocalPos;
                newLocalPos.x = -Mathf.Abs(attackOriginInitialLocalPos.x);
                attackOrigin.localPosition = newLocalPos;
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Play jump sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound2D("Player Jump");
            }
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

    void HandleFootstepSounds()
    {
        // Play running sound with cooldown
        if (Mathf.Abs(move) > 0.1f && isGrounded && Time.time > lastFootstepTime + footstepCooldown)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound2D("Player Run");
            }
            lastFootstepTime = Time.time;
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            Debug.Log("ATTACK!");

            canAttack = false;

            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

            Debug.Log("Enemies hit: " + hitEnemies.Length);

            foreach (Collider2D enemy in hitEnemies)
            {
                Bat bat = enemy.GetComponentInParent<Bat>();

                if (bat != null)
                {
                    Debug.Log("Hitting Bat!");
                    bat.TakeDamage(attackDamage);
                    continue;
                }

                Spider spider = enemy.GetComponentInParent<Spider>();

                if (spider != null)
                {
                    Debug.Log("Hitting Spider!");
                    spider.TakeDamage(attackDamage);
                    continue;
                }

                Boss boss = enemy.GetComponentInParent<Boss>();

                if (boss != null)
                {
                    Debug.Log("Hitting Boss!");
                    boss.TakeDamage(attackDamage);
                }
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
        if (anim == null || isDead) return;

        // Use the original parameter names from your first script
        anim.SetFloat("Speed", Mathf.Abs(move));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        healthScript.TakeDamage(damage);

        if (healthScript.currentHealth > 0)
        {
            if (anim != null)
            {
                anim.SetTrigger("Hurt");
            }
        }
        else
        {
            Die(); 
        }

        // Play hurt sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D("Player Hurt");
        }

        Debug.Log("Player HP: " + healthScript.currentHealth);

        if (healthScript.currentHealth <= 0)
        {
            Die();
        }
    }

    void DisableAnimator()
    {
        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("PLAYER DIED!");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;

        if (anim != null)
        {
            anim.Play("DeathEffect");
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        Destroy(gameObject, 1f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Lava"))
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Gem collection with audio
        if (collision.CompareTag("Gem") && collision.gameObject.activeSelf == true)
        {
            collision.gameObject.SetActive(false);
            gemCounter += 1;
            counterText.text = $"Gems: {gemCounter}/{totalGems}";

            // Play gem pickup sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound2D("Gem Pickup");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
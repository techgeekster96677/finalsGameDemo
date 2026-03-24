using TMPro;
using UnityEngine;

// Main player controller that handles movement, jumping, attacking,
// health interaction, UI updates, and environmental interactions.
public class PlayerWarrior : MonoBehaviour
{
    [Header("Movement")]
    // Horizontal movement speed multiplier
    public float speed = 5f;

    // Upward force applied when jumping
    public float jumpForce = 10f;

    [Header("Ground Check")]
    // Position used to check if the player is touching the ground
    public Transform groundCheck;

    // Radius of the circular ground detection area
    public float groundCheckRadius = 0.5f;

    // Defines which layers are considered ground
    public LayerMask groundLayer;

    [Header("Attack")]
    // Center point of the attack detection area
    public Transform attackPoint;

    // Radius of the attack hitbox
    public float attackRange = 4f;

    // Defines which layers are considered enemies
    public LayerMask enemyLayer;

    // Damage applied per attack
    public int attackDamage = 1;

    // Reference to reusable Health system
    private Health healthScript;

    // UI screen shown when player dies
    public GameObject gameOverScreen;

    [Header("Attack Origin")]
    // Used to reposition attack direction depending on where the player faces
    [SerializeField] private Transform attackOrigin;

    // Stores original position of attack origin for mirroring
    private Vector3 attackOriginInitialLocalPos;

    // Core Unity components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Movement and state tracking
    private bool isGrounded;
    private bool isDead = false;

    // Stores horizontal input value (-1 left, 1 right, 0 idle)
    private float move;

    // Controls timing of footstep sounds
    private float lastFootstepTime = 0f;
    public float footstepCooldown = 0.3f;

    // Gem tracking and UI display
    private int gemCounter = 0;
    public int totalGems = 0;
    public TextMeshProUGUI counterText;

    // Prevents continuous attack spamming
    private bool canAttack = true;
    public float attackCooldown = 0.2f;

    void Start()
    {
        // Cache frequently used components for performance
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<Health>();

        // Store the original local position of the attack origin
        // This allows mirroring when the player flips direction
        if (attackOrigin != null)
        {
            attackOriginInitialLocalPos = attackOrigin.localPosition;
        }

        // Automatically detect total gems in the scene if not manually set
        if (totalGems <= 0)
        {
            totalGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        }

        // Initialize gem counter UI
        counterText.text = $"Gems: {gemCounter}/{totalGems}";
    }

    void Update()
    {
        // Prevent any input or actions if player is dead
        if (isDead) return;

        // Reads player input continuously
        // Returns a smoothed value between -1 and 1
        move = Input.GetAxis("Horizontal");

        Move();
        Jump();
        Attack();
        HandleAnimation();
        HandleFootstepSounds();
    }

    void FixedUpdate()
    {
        // Ground detection is placed in FixedUpdate to stay in sync with physics updates
        CheckGround();
    }

    // Handles horizontal movement using Rigidbody2D physics
    void Move()
    {
        // The player's movement is controlled by directly modifying velocity.
        // The X-axis velocity is determined by input (move) multiplied by speed.
        // The Y-axis velocity is preserved so gravity and jumping are not affected.

        rb.linearVelocity = new Vector2(
            move * speed,
            rb.linearVelocity.y
        );

        /*
        Movement behavior:
        - move = 1  → velocity.x becomes positive → player moves right
        - move = -1 → velocity.x becomes negative → player moves left
        - move = 0  → velocity.x becomes zero → player stops

        This approach uses Unity’s physics system instead of directly changing position,
        resulting in smoother and more realistic motion.
        */

        // Adjust player facing direction by flipping the sprite horizontally
        if (move > 0)
        {
            // Ensure sprite faces right
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );

            // Position the attack origin in front of the player (right side)
            if (attackOrigin != null)
            {
                Vector3 newLocalPos = attackOriginInitialLocalPos;
                newLocalPos.x = Mathf.Abs(attackOriginInitialLocalPos.x);
                attackOrigin.localPosition = newLocalPos;
            }
        }
        else if (move < 0)
        {
            // Flip sprite to face left
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );

            // Mirror attack origin to the left side
            if (attackOrigin != null)
            {
                Vector3 newLocalPos = attackOriginInitialLocalPos;
                newLocalPos.x = -Mathf.Abs(attackOriginInitialLocalPos.x);
                attackOrigin.localPosition = newLocalPos;
            }
        }
    }

    // Handles jumping using vertical velocity
    void Jump()
    {
        // Jump only occurs when player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Sets upward velocity directly
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            // Gravity will automatically pull the player back down afterward
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound2D("Player Jump");
            }
        }
    }

    // Detects whether the player is on the ground
    void CheckGround()
    {
        // Uses a circular overlap check to detect ground objects
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    // Plays footstep sounds while the player is moving on the ground
    void HandleFootstepSounds()
    {
        /*
        This condition ensures that footsteps only play under controlled situations:

        1. Mathf.Abs(move) > 0.1f
           - Checks if the player is actually moving horizontally
           - move ranges from -1 to 1, so values near 0 mean idle
           - 0.1f acts as a threshold to avoid tiny inputs triggering sound

        2. isGrounded
           - Prevents footstep sounds while in the air (jumping/falling)

        3. Time.time > lastFootstepTime + footstepCooldown
           - Ensures sound does not play every frame
           - Time.time is the current game time
           - lastFootstepTime stores when the last step sound played
           - footstepCooldown controls delay between sounds
        */
        if (Mathf.Abs(move) > 0.1f && isGrounded && Time.time > lastFootstepTime + footstepCooldown)
        {
            // Check if SoundManager exists to avoid null reference errors
            if (SoundManager.Instance != null)
            {
                // Play running sound effect
                SoundManager.Instance.PlaySound2D("Player Run");
            }

            // Update last played time so cooldown can be enforced
            lastFootstepTime = Time.time;
        }
    }


    // Handles attack detection and applies damage to enemies
    void Attack()
    {
        /*
        Input.GetKeyDown(KeyCode.E)
        - Detects a single key press (not continuous holding)

        canAttack
        - Prevents spamming attacks
        - Works together with cooldown system
        */
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            // Immediately disable attack until cooldown finishes
            canAttack = false;

            // Trigger attack animation
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }

            /*
            Physics2D.OverlapCircleAll:
            - Creates an invisible circular detection area
            - Center = attackPoint.position
            - Radius = attackRange
            - Filters objects using enemyLayer

            Returns:
            - Array of all colliders inside that area
            */
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

            /*
            Each detected collider is checked to determine what type of enemy it belongs to.

            GetComponentInParent is used instead of GetComponent because:
            - The collider might be on a child object (e.g., hitbox)
            - The actual script (Bat, Spider, Boss) is usually on the parent object
            */
            foreach (Collider2D enemy in hitEnemies)
            {
                // Check if this collider belongs to a Bat enemy
                Bat bat = enemy.GetComponentInParent<Bat>();
                if (bat != null)
                {
                    bat.TakeDamage(attackDamage);
                    continue; // Skip remaining checks once matched
                }

                // Check if this collider belongs to a Spider enemy
                Spider spider = enemy.GetComponentInParent<Spider>();
                if (spider != null)
                {
                    spider.TakeDamage(attackDamage);
                    continue;
                }

                // Check if this collider belongs to a Boss enemy
                Boss boss = enemy.GetComponentInParent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(attackDamage);
                }
            }

            /*
            Invoke delays the execution of ResetAttack method.
            This creates a cooldown system without using timers manually.

            attackCooldown determines how long the player must wait before attacking again.
            */
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }


    // Re-enables attacking after cooldown finishes
    void ResetAttack()
    {
        // Allows player to attack again
        canAttack = true;
    }


    // Updates animation parameters based on movement and physics state
    void HandleAnimation()
    {
        // Prevent errors and unnecessary updates if animator is missing or player is dead
        if (anim == null || isDead) return;

        /*
        Speed parameter:
        - Uses absolute value so left/right movement both trigger running animation
        - 0 = idle, >0 = moving
        */
        anim.SetFloat("Speed", Mathf.Abs(move));

        /*
        isGrounded parameter:
        - Used to switch between jump/fall and grounded animations
        */
        anim.SetBool("isGrounded", isGrounded);

        /*
        yVelocity parameter:
        - Positive value → jumping upward
        - Negative value → falling downward
        - Helps animator determine correct airborne animation
        */
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }


    // Applies damage to the player through the Health system
    public void TakeDamage(int damage)
    {
        // Prevent damage if player is already dead
        if (isDead) return;

        // Delegate actual health reduction to Health script
        healthScript.TakeDamage(damage);

        /*
        Check remaining health:
        - If still alive → play hurt animation
        - If zero → trigger death
        */
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

        // Play hurt sound effect if sound system exists
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound2D("Player Hurt");
        }
    }


    // Handles all player death logic
    void Die()
    {
        // Prevent multiple executions of death logic
        if (isDead) return;

        isDead = true;

        /*
        Stop all movement immediately:
        - Sets velocity to zero
        - Prevents sliding or continued motion
        */
        rb.linearVelocity = Vector2.zero;

        /*
        Disable physics interactions:
        - Setting body type to Static prevents forces and collisions from affecting the player
        */
        rb.bodyType = RigidbodyType2D.Static;

        /*
        Disable collider:
        - Prevents further collisions (enemies, ground, etc.)
        */
        GetComponent<Collider2D>().enabled = false;

        // Play death animation
        if (anim != null)
        {
            anim.Play("DeathEffect");
        }

        // Activate Game Over UI
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        /*
        Destroy player after a delay:
        - Allows time for death animation to play before removal
        */
        Destroy(gameObject, 1f);
    }


    // Detects collision with solid hazards like lava
    void OnCollisionEnter2D(Collision2D col)
    {
        // Collision (not trigger) is used for physical objects like lava platforms
        if (col.gameObject.CompareTag("Lava"))
        {
            Die();
        }
    }


    // Handles trigger-based interactions such as collectibles
    void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        Conditions:
        - Object must be tagged as "Gem"
        - Object must still be active (prevents double collection)
        */
        if (collision.CompareTag("Gem") && collision.gameObject.activeSelf == true)
        {
            /*
            Instead of destroying the object, it is deactivated:
            - More efficient (object pooling friendly)
            - Avoids unnecessary memory allocation
            */
            collision.gameObject.SetActive(false);

            // Increase gem count
            gemCounter += 1;

            // Update UI text immediately
            counterText.text = $"Gems: {gemCounter}/{totalGems}";

            // Play collection sound
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound2D("Gem Pickup");
            }
        }
    }


    // Draws visual guides in the Unity Editor for debugging
    void OnDrawGizmosSelected()
    {
        // Prevent drawing if attack point is not assigned
        if (attackPoint == null) return;

        /*
        Draw attack range:
        - Red wire sphere shows where attacks will hit enemies
        */
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        /*
        Draw ground check area:
        - Green wire sphere shows where ground detection occurs
        */
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
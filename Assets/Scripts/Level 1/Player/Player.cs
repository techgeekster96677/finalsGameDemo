using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Controls player movement, jumping, animations, footstep sounds, and gem collection.
/// Handles character flipping and attack origin positioning based on movement direction.
/// </summary>
public class Player : MonoBehaviour
{
    // Player movement parameters
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // New field for attack origin
    [SerializeField] private Transform attackOrigin;
    private Vector3 attackOriginInitialLocalPos;

    // Player state components
    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Footstep sound cooldown
    private float lastFootstepTime = 0f;
    public float footstepCooldown = 0.3f;

    // Gem collection
    private int gemCounter = 0;
    public int totalGems = 0; // set per-level in inspector, or leave 0 to auto-count
    public TextMeshProUGUI counterText;

    /// <summary>
    /// Initializes player components, animator state, and gem counter.
    /// Stores initial attack origin position for flipping logic.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Store the initial local position of attack origin
        if (attackOrigin != null)
        {
            attackOriginInitialLocalPos = attackOrigin.localPosition;
        }

        // Initialize grounded state
        UpdateGroundedState();

        // Reset all animator parameters to ensure clean start
        animator.SetBool("run", false);
        animator.SetBool("grounded", isGrounded);

        // Make sure we're not triggering any jump animation at start
        animator.ResetTrigger("jump");

        // Initialize gem counter UI
        if (totalGems <= 0)
        {
            totalGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        }

        counterText.text = $"Gems: {gemCounter}/{totalGems}";
    }

    /// <summary>
    /// Handles player input for movement, jumping, and flipping.
    /// Updates animator parameters and plays footstep sounds with cooldown.
    /// </summary>
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        // Movement
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // Flip character based on movement direction AND update attack origin
        if (moveX > 0)
        {
            spriteRenderer.flipX = false;
            // Update attack origin position when facing right
            if (attackOrigin != null)
            {
                Vector3 newLocalPos = attackOriginInitialLocalPos;
                newLocalPos.x = Mathf.Abs(attackOriginInitialLocalPos.x);
                attackOrigin.localPosition = newLocalPos;
            }
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true;
            // Update attack origin position when facing left
            if (attackOrigin != null)
            {
                Vector3 newLocalPos = attackOriginInitialLocalPos;
                newLocalPos.x = -Mathf.Abs(attackOriginInitialLocalPos.x);
                attackOrigin.localPosition = newLocalPos;
            }
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Update animator parameters
        animator.SetBool("run", moveX != 0);
        animator.SetBool("grounded", isGrounded);

        // Play running sound with cooldown
        if (moveX != 0 && isGrounded && Time.time > lastFootstepTime + footstepCooldown)
        {
            SoundManager.Instance.PlaySound2D("Player Run");
            lastFootstepTime = Time.time;
        }
    }

    /// <summary>
    /// Updates grounded state in FixedUpdate for physics consistency.
    /// </summary>
    void FixedUpdate()
    {
        UpdateGroundedState();
    }

    /// <summary>
    /// Checks if the player is grounded using a circle cast at the ground check point.
    /// Updates animator grounded state accordingly.
    /// </summary>
    private void UpdateGroundedState()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Also update animator in FixedUpdate to ensure it stays in sync
        if (animator != null)
        {
            animator.SetBool("grounded", isGrounded);
        }
    }

    /// <summary>
    /// Applies jump force, triggers jump animation, and plays jump sound.
    /// </summary>
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        SoundManager.Instance.PlaySound2D("Player Jump");
    }

    /// <summary>
    /// Handles gem collection when the player enters a gem trigger.
    /// Deactivates the gem, increments counter, updates UI, and plays pickup sound.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is a gem and is active
        if (collision.CompareTag("Gem") && collision.gameObject.activeSelf == true)
        {
            collision.gameObject.SetActive(false);                  // Deactivate the gem instead of destroying it
            gemCounter += 1;                                        // Increment the gem counter
            counterText.text = $"Gems: {gemCounter}/{totalGems}";   // Update the UI text with the new gem count
            SoundManager.Instance.PlaySound2D("Gem Pickup");        // Play the gem pickup sound effect
        }
    }
}
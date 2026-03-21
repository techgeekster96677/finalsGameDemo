using UnityEngine;
using System.Collections;
using TMPro;

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

    void FixedUpdate()
    {
        UpdateGroundedState();
    }

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

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        SoundManager.Instance.PlaySound2D("Player Jump");
    }

    // Optional: Visualize the ground check in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

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
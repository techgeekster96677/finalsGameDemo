using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Add this for attack origin
    [Header("Attack")]
    [SerializeField] private Transform attackOrigin;
    private Vector3 attackOriginInitialLocalPos;

    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

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
}
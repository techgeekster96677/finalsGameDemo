using UnityEngine;

/// <summary>
/// Controls enemy patrol behavior between two defined edges.
/// Handles movement, direction changes, idle pauses at edges.
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement Settings")]
    [SerializeField] private float speed;
    private Vector3 initialScale;
    private bool movingLeft = true;

    [Header("Idle Settings")]
    [SerializeField] private float idleDuration; // to wait at edges
    private float idleTimer;

    [Header("Animations")]
    [SerializeField] private Animator animator;

    /// <summary>
    /// Initializes enemy reference and stores initial scale for flipping.
    /// </summary>
    private void Awake()
    {
        if (enemy == null)
            enemy = transform;

        initialScale = enemy.localScale;
    }

    /// <summary>
    /// Updates patrol movement based on current direction and edge boundaries.
    /// Moves left until reaching left edge, then changes direction.
    /// Moves right until reaching right edge, then changes direction.
    /// </summary>
    /// 
    // 3/15/2026: Fixed enemy patrol issues - background script was preventing movement.
    // 3/16/2026: Enemy now properly attacks player. Adjusted hitbox so player doesn't get hurt too easily.
    private void Update()
    {
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
            else
            {
                ChangeDirection();
            }
        }
        else // moving right
        {
            if (enemy.position.x <= rightEdge.position.x)
            {
                MoveInDirection(1);
            }
            else
            {
                ChangeDirection();
            }
        }

        // REMOVED THE BUG: MoveInDirection(1); was here causing constant right movement
    }

    /// <summary>
    /// Called when the script is disabled (e.g., when enemy detects player).
    /// Stops movement animation to prevent visual glitches.
    /// </summary>
    private void OnDisable()
    {
        animator.SetBool("moving", false); // Stop moving animation when disabled
    }

    /// <summary>
    /// Changes patrol direction after waiting at an edge for idleDuration.
    /// Stops movement animation during the idle pause.
    /// </summary>
    private void ChangeDirection()
    {
        // Stop moving animation
        if (animator != null)
            animator.SetBool("moving", false);

        // Wait at edge
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0f; // Reset timer after changing direction
        }
    }

    /// <summary>
    /// Moves the enemy in the specified direction.
    /// 
    /// BEHAVIOR:
    /// - Resets idle timer
    /// - Triggers movement animation
    /// - Flips sprite based on direction (negative scale for left, positive for right)
    /// - Updates position based on speed and delta time
    /// </summary>
    private void MoveInDirection(int direction)
    {
        // Reset idle timer when moving
        idleTimer = 0;

        // Play moving animation
        if (animator != null)
            animator.SetBool("moving", true);

        // Flip enemy sprite based on direction
        // For left movement (direction -1), use negative scale
        // For right movement (direction 1), use positive scale
        enemy.localScale = new Vector3(
            Mathf.Abs(initialScale.x) * direction,
            initialScale.y,
            initialScale.z
        );

        // Move enemy
        enemy.position = new Vector3(
            enemy.position.x + Time.deltaTime * direction * speed,
            enemy.position.y,
            enemy.position.z
        );
    }
}
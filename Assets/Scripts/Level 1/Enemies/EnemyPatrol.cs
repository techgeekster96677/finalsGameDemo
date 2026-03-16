using UnityEngine;

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

    private void Awake()
    {
        if (enemy == null)
            enemy = transform;

        initialScale = enemy.localScale;
    }

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

    private void OnDisable()
    {
        animator.SetBool("moving", false); // Stop moving animation when disabled
    }

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

    // Optional: Visualize patrol edges in editor
    private void OnDrawGizmosSelected()
    {
        if (leftEdge != null && rightEdge != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftEdge.position, rightEdge.position);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leftEdge.position, 0.2f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rightEdge.position, 0.2f);
        }
    }
}
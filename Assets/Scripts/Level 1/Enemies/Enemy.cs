using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private LayerMask playerLayer;

    [Header("Touch Damage")]
    [SerializeField] private bool damageOnTouch = true; // Enable damage when player touches enemy
    [SerializeField] private float touchDamageCooldown = 0.5f; // Prevent constant damage every frame

    private float cooldownTimer = Mathf.Infinity;
    private float touchDamageTimer = 0f;

    //References
    private Animator animator;
    private Health playerHealth;
    private bool isAttacking = false;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();

        if (boxCollider2D == null)
            boxCollider2D = GetComponent<BoxCollider2D>();

        if (boxCollider2D == null)
            Debug.LogWarning("Enemy script requires a BoxCollider2D component on the GameObject!", this);
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;
        touchDamageTimer += Time.deltaTime;

        // Only check for player if we have a boxCollider2D
        if (boxCollider2D != null && PlayerInSight())
        {
            // Stop patrol when player is detected
            if (enemyPatrol != null)
                enemyPatrol.enabled = false;

            if (cooldownTimer >= attackCooldown && !isAttacking)
            {
                isAttacking = true;
                cooldownTimer = 0f;
                animator.SetTrigger("meleeAttack");
            }
        }
        else
        {
            // Resume patrol when player is out of sight or no collider
            if (enemyPatrol != null)
                enemyPatrol.enabled = !PlayerInSight();
        }

        // Check for touch damage (player trying to go through enemy)
        CheckTouchDamage();
    } 

    private void CheckTouchDamage()
    {
        if (!damageOnTouch) return;

        // Check if player is touching the enemy's collider
        Collider2D playerCollider = Physics2D.OverlapBox(
            boxCollider2D.bounds.center,
            boxCollider2D.bounds.size,
            0,
            playerLayer
        );

        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            // Apply damage on cooldown
            if (touchDamageTimer >= touchDamageCooldown)
            {
                Health playerHealthComponent = playerCollider.GetComponent<Health>();
                if (playerHealthComponent != null)
                {
                    playerHealthComponent.TakeDamage(damage);
                    Debug.Log("Player damaged by touching enemy! Health: " + playerHealthComponent.currentHealth);
                    touchDamageTimer = 0f;
                }
            }
        }
    }

    // This method should be called via Animation Event
    public void DamagePlayer()
    {
        // If player is still in sight after attack animation, apply damage
        if (boxCollider2D != null && PlayerInSight() && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            //Debug.Log("Player attacked for " + damage + " damage! Player health: " + playerHealth.currentHealth);
        }
        else
        {
            //Debug.Log("Player not in sight or playerHealth is null");
        }
    }

    // This method should be called via Animation Event at the end of attack animation
    public void AttackFinished()
    {
        isAttacking = false;
    }

    private bool PlayerInSight()
    {
        // Safety check
        if (boxCollider2D == null)
            return false;

        // Calculate direction based on enemy facing
        float direction = transform.localScale.x > 0 ? 1 : -1;

        // Calculate the box cast position
        Vector3 boxCastPosition = boxCollider2D.bounds.center +
                                  new Vector3(direction * range * colliderDistance, 0, 0);

        Vector3 boxCastSize = new Vector3(
            boxCollider2D.bounds.size.x * range,
            boxCollider2D.bounds.size.y,
            boxCollider2D.bounds.size.z
        );

        // Perform the box cast
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCastPosition,
            boxCastSize,
            0,
            Vector2.zero,
            0,
            playerLayer
        );

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            //Debug.Log("Player in sight!");
            playerHealth = hit.transform.GetComponent<Health>();
            return true;
        }

        return false;
    }

    // Visualize both attack area and enemy collider
    private void OnDrawGizmos()
    {
        if (boxCollider2D == null)
            return;

        // Draw attack area in red
        Gizmos.color = Color.red;
        float direction = transform.localScale.x > 0 ? 1 : -1;
        Vector3 attackAreaPosition = boxCollider2D.bounds.center +
                                     new Vector3(direction * range * colliderDistance, 0, 0);
        Vector3 attackAreaSize = new Vector3(
            boxCollider2D.bounds.size.x * range,
            boxCollider2D.bounds.size.y,
            boxCollider2D.bounds.size.z
        );
        Gizmos.DrawWireCube(attackAreaPosition, attackAreaSize);

        // Draw enemy collider in blue (touch damage area)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxCollider2D.bounds.center, boxCollider2D.bounds.size);
    }
}
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player melee attack logic with cooldown and animation events.
/// Uses an attack origin point and radius to detect enemies within range.
/// </summary>
public class PlayerMelee : MonoBehaviour
{
    public Transform attackOrigin;
    public float attackRadius = 1f;
    public LayerMask enemyLayer;
    public int attackDamage;
    public float cooldownTime = .5f;

    private float cooldownTimer = 0f;
    [SerializeField] private Animator animator;

    /// <summary>
    /// Updates cooldown timer and checks for attack input.
    /// Attack is triggered when cooldown is ready and E key is pressed.
    /// </summary>
    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0 && Input.GetKeyDown(KeyCode.E))
        {
            PerformAttack();
        }
    }

    /// <summary>
    /// Triggers the melee attack animation and resets the cooldown timer.
    /// Actual damage is applied via animation event to sync with attack animation timing.
    /// </summary>
    private void PerformAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("meleeAttack");
        }
        cooldownTimer = cooldownTime;
    }

    /// <summary>
    /// Called by Animation Event during the attack animation.
    /// Detects all enemies within the attack radius and applies damage to them.
    /// </summary>
    public void DamageEnemiesInRange()
    {
        Debug.Log("Attack event triggered!");

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyLayer);

        if (enemiesInRange.Length == 0)
        {
            Debug.Log("No enemies in range");
            return;
        }

        foreach (var enemy in enemiesInRange)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("Hit: " + enemy.name);
            }
        }
    }

    /// <summary>
    /// Visualizes the attack radius in the Scene view for debugging.
    /// Red wire sphere shows the area where enemies will be detected.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
        }
    }
}
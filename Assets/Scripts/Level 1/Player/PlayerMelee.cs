using UnityEngine;
using System.Collections;

public class PlayerMelee : MonoBehaviour
{
    public Transform attackOrigin;
    public float attackRadius = 1f;
    public LayerMask enemyLayer;
    public int attackDamage;
    public float cooldownTime = .5f;

    private float cooldownTimer = 0f;
    [SerializeField] private Animator animator;

    // For debugging - shows attack range in Scene view
    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0 && Input.GetKeyDown(KeyCode.E))
        {
            PerformAttack();
            SoundManager.Instance.PlaySound2D("Player Attack");
        }
    }

    private void PerformAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("meleeAttack");
        }
        cooldownTimer = cooldownTime;
    }

    // Called by Animation Event
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

    private void OnDrawGizmos()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
        }
    }
}
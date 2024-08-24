using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab;  // Prefab of the projectile to shoot
    public float projectileSpeed = 10f;  // Speed of the projectile
    public bool disableMeleeAttack = true;  // Option to disable melee attacks when using ranged attacks
    public float rangedAttackRange = 5.0f; // Range for ranged attacks
    public int maxTargets = 3;           // Maximum number of enemies to attack at once

    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();

        // Optionally disable melee attack
        if (disableMeleeAttack)
        {
            unit.attackRange = Mathf.Max(unit.attackRange, rangedAttackRange);
        }
    }

    private void Update()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, rangedAttackRange, unit.enemyLayer);

        if (enemiesInRange.Length > 0 && Time.time >= unit.LastAttackTime + unit.attackCooldown)
        {
            ShootProjectilesAtMultipleEnemies(enemiesInRange);
            unit.LastAttackTime = Time.time;  // Reset the cooldown timer
        }
    }

    private void ShootProjectilesAtMultipleEnemies(Collider2D[] enemiesInRange)
    {
        int targetsAttacked = 0;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            if (targetsAttacked >= maxTargets)
                break;

            GameObject enemy = enemyCollider.gameObject;
            if (enemy != null)
            {
                ShootProjectile(enemy);
                targetsAttacked++;
            }
        }

        Debug.Log($"{gameObject.name} shot at {targetsAttacked} enemies.");
    }

    private void ShootProjectile(GameObject target)
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Ensure the projectile has a Rigidbody2D component
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = projectile.AddComponent<Rigidbody2D>();
            }

            // Set the velocity of the projectile
            Vector2 direction = (target.transform.position - transform.position).normalized;
            rb.velocity = direction * projectileSpeed;

            // Optional: Set gravity scale to 0 if it's not meant to fall
            rb.gravityScale = 0;

            // Set enemy layer on the projectile
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.enemyLayer = unit.enemyLayer;  // Set the enemy layer from the Unit
            }

            Debug.Log($"{gameObject.name} shoots a projectile towards {target.name}.");
        }
    }
}

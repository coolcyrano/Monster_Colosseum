using UnityEngine;

public class RangedMultiAttack : MonoBehaviour
{
    public GameObject projectilePrefab;  // Prefab of the projectile to shoot
    public float projectileSpeed = 10f;  // Speed of the projectile
    public int maxTargets = 3;           // Maximum number of enemies to attack at once

    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    private void Update()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, unit.attackRange, unit.enemyLayer);

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

            Vector2 direction = (target.transform.position - transform.position).normalized;

            // Initialize the projectile with the direction towards the target
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(direction);
            }

            Debug.Log($"{gameObject.name} shoots a projectile towards {target.name}.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttack : MonoBehaviour
{
    public int maxTargets = 3;  // Max number of targets to attack
    public float attackRadius = 2f;  // Radius within which enemies are detected
    public int damagePerAttack = 10;  // Damage dealt per attack

    private void Start()
    {
        // Initialize MultiAttack or perform any setup needed
        Debug.Log($"MultiAttack initialized for {gameObject.name}");
    }

    private void Update()
    {
        AttackEnemies();
    }

    private void AttackEnemies()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRadius, LayerMask.GetMask("Enemy"));
        Debug.Log($"Enemies detected: {enemiesInRange.Length} for {gameObject.name}");

        // Make sure to limit the number of targets to attack
        int targetsAttacked = 0;
        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            if (targetsAttacked >= maxTargets)
                break;

            Unit enemyUnit = enemyCollider.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                // Apply damage
                enemyUnit.TakeDamage(damagePerAttack);
                Debug.Log($"{gameObject.name} attacked {enemyUnit.gameObject.name} for {damagePerAttack} damage.");
                targetsAttacked++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}


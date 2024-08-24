using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttack : MonoBehaviour
{
    public int maxTargets = 3;  // Maximum number of enemies to attack at once
    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();

        if (unit == null)
        {
            Debug.LogError("Unit component not found on " + gameObject.name);
            return;
        }

        Debug.Log($"MultiAttack initialized for {gameObject.name}");
        StartCoroutine(DelayedStart()); // Ensure initialization
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure initialization
    }

    private void Update()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, unit.attackRange, unit.enemyLayer);

        Debug.Log($"Enemies detected: {enemiesInRange.Length} for {gameObject.name}");

        if (enemiesInRange.Length > 0 && Time.time >= unit.LastAttackTime + unit.attackCooldown)
        {
            AttackMultipleEnemies(enemiesInRange);
            unit.LastAttackTime = Time.time;  // Reset the cooldown timer
        }
    }

    private void AttackMultipleEnemies(Collider2D[] enemiesInRange)
    {
        int targetsAttacked = 0;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            if (targetsAttacked >= maxTargets)
                break;

            Unit enemyUnit = enemyCollider.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                unit.Attack(enemyUnit);  // Ensure this method handles each attack properly
                targetsAttacked++;
                Debug.Log($"{gameObject.name} attacked {enemyUnit.gameObject.name}. Total targets attacked: {targetsAttacked}");
            }
        }

        Debug.Log($"{gameObject.name} attacked {targetsAttacked} enemies.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unit.attackRange);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float moveSpeed = 1f;
    public int health = 100;
    public int damage = 10;
    public float attackRange = 1.0f;
    public float separationDistance = 1.0f;  // Minimum distance between units
    public float avoidanceStrength = 1.0f;  // Strength of the avoidance behavior
    public LayerMask enemyLayer;  // Layer of the enemies
    public LayerMask allyLayer;   // Layer of allied units
    public float attackCooldown = 1.0f;  // Cooldown duration in seconds
    public int cost = 10;

    private float lastAttackTime;
    private Vector2 minBounds;  // Minimum screen bounds
    private Vector2 maxBounds;  // Maximum screen bounds
    private Animator animator;  // Reference to the Animator component

    // Property to expose LastAttackTime to other scripts
    public float LastAttackTime
    {
        get { return lastAttackTime; }
        set { lastAttackTime = value; }
    }

    // Property to expose the Animator to other scripts
    public Animator Animator
    {
        get { return animator; }
    }

    private void Start()
    {
        lastAttackTime = -attackCooldown;  // Allow immediate first attack

        // Calculate screen bounds
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ScreenToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Find the closest enemy on the opposing team
        GameObject closestEnemy = FindClosestEnemy();

        if (closestEnemy != null)
        {
            // Calculate the distance to the closest enemy
            float distanceToEnemy = Vector2.Distance(transform.position, closestEnemy.transform.position);

            // Stop moving and attack if within attack range
            if (distanceToEnemy <= attackRange)
            {
                // Attack the enemy if the attack cooldown has passed
                if (Time.time >= LastAttackTime + attackCooldown)
                {
                    Attack(closestEnemy.GetComponent<Unit>());
                    LastAttackTime = Time.time;  // Reset the cooldown timer
                }
            }
            else
            {
                // Move towards the closest enemy while avoiding collisions with other units
                MoveTowardsEnemy(closestEnemy);
            }
        }

        // Destroy unit if health is depleted
        if (health <= 0)
        {
            DestroyUnit();
        }
    }

    private void MoveTowardsEnemy(GameObject closestEnemy)
    {
        Vector2 targetDirection = (closestEnemy.transform.position - transform.position).normalized;
        Vector2 avoidanceVector = CalculateAvoidanceVector();

        // Combine the target direction with the avoidance vector
        Vector2 movementDirection = targetDirection + avoidanceVector;
        movementDirection.Normalize();  // Re-normalize the direction vector

        // Move the unit
        Vector2 newPosition = (Vector2)transform.position + movementDirection * moveSpeed * Time.deltaTime;
        newPosition = ConstrainToBounds(newPosition);
        transform.position = newPosition;

        // Flip the sprite to face the opponent
        FlipSprite(closestEnemy.transform.position.x);
    }

    private Vector2 CalculateAvoidanceVector()
    {
        Vector2 avoidanceVector = Vector2.zero;
        Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, separationDistance, allyLayer);

        foreach (Collider2D unitCollider in nearbyUnits)
        {
            if (unitCollider.gameObject != gameObject)
            {
                Vector2 toOtherUnit = (Vector2)unitCollider.transform.position - (Vector2)transform.position;
                float distance = toOtherUnit.magnitude;

                if (distance < separationDistance && distance > 0.01f)
                {
                    // Calculate a vector perpendicular to the direction of the other unit
                    Vector2 avoidanceDir = Vector2.Perpendicular(toOtherUnit.normalized);
                    avoidanceVector += avoidanceDir * (avoidanceStrength / distance); // Influence of avoidance behavior
                }
            }
        }

        return avoidanceVector;
    }

    private Vector2 ConstrainToBounds(Vector2 position)
    {
        // Constrain the position within the screen bounds
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return position;
    }

    private void FlipSprite(float targetX)
    {
        float xDifference = targetX - transform.position.x;
        if (Mathf.Abs(xDifference) > 0.1f)
        {
            transform.localScale = new Vector3(xDifference > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void Attack(Unit enemyUnit)
    {
        if (enemyUnit != null)
        {
            // Trigger the attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            enemyUnit.TakeDamage(damage);
            Debug.Log($"{gameObject.name} attacked {enemyUnit.gameObject.name} for {damage} damage.");
        }
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"{gameObject.name} takes {amount} damage. Health is now {health - amount}.");
        health -= amount;
    }

    private void DestroyUnit()
    {
        Debug.Log($"{gameObject.name} has been destroyed.");

        // Notify the EnemyManager to remove this enemy from the list
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.RemoveEnemy(gameObject);
        }

        // Destroy this unit's GameObject
        Destroy(gameObject);
    }

    public GameObject FindClosestEnemy()
    {
        // Only look for enemies on the specified enemy layer
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, Mathf.Infinity, enemyLayer);
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemyCollider.gameObject;
            }
        }

        return closestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack range and separation distance in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}


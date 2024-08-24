using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;          // Damage dealt by the projectile
    public float lifeTime = 5f;      // Time after which the projectile is destroyed
    public float speed = 10f;        // Speed of the projectile
    public LayerMask enemyLayer;     // Layer to detect enemies

    private Vector2 direction;

    private void Start()
    {
        // Destroy the projectile after its lifetime to avoid memory leaks
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Move the projectile in the set direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // Initialize the direction of the projectile
    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is on the enemy layer
        if ((enemyLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            // Attempt to get the Unit component from the collided object
            Unit enemyUnit = collision.GetComponent<Unit>();

            if (enemyUnit != null)
            {
                // Apply damage to the enemy unit
                enemyUnit.TakeDamage(damage);
                Debug.Log($"Projectile hit {enemyUnit.gameObject.name}, dealing {damage} damage.");

                // Destroy the projectile after hitting the enemy
                Destroy(gameObject);
            }
        }
    }
}

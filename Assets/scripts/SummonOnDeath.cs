using UnityEngine;

public class SummonOnDeath : MonoBehaviour
{
    public GameObject summonPrefab;  // Prefab to summon on death (e.g., a bullet prefab)
    public int summonCount = 3;      // Number of prefabs to summon
    public float spawnRadius = 1.0f; // Radius around the unit where prefabs will be spawned

    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    private void OnDestroy()
    {
        // Only summon if the unit was destroyed due to health reaching zero
        if (unit.health <= 0 && summonPrefab != null)
        {
            for (int i = 0; i < summonCount; i++)
            {
                Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
                GameObject projectile = Instantiate(summonPrefab, spawnPosition, Quaternion.identity);

                // Randomize the projectile's direction
                Vector2 randomDirection = Random.insideUnitCircle.normalized;

                // Initialize the projectile with the random direction
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.Initialize(randomDirection);
                }

                Debug.Log($"{gameObject.name} summoned {summonPrefab.name} at position {spawnPosition} with direction {randomDirection}.");
            }
        }
    }
}

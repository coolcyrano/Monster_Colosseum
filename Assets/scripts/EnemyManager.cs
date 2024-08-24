using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load the level selector scene

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies; // List of enemies in the level

    private void Update()
    {
        // Check if all enemies are defeated
        if (enemies.Count == 0)
        {
            // Load the level selector scene
            SceneManager.LoadScene("LevelSelector");
            Debug.Log("All enemies defeated, loading level selector.");
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Destroy(enemy);

            if (enemies.Count == 0)
            {
                // Load the level selector scene
                SceneManager.LoadScene("LevelSelector");
                Debug.Log("All enemies defeated, loading level selector.");
            }
        }
        else
        {
            Debug.LogWarning("Attempted to remove enemy not in list: " + enemy.name);
        }
    }
}


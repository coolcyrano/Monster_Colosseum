using UnityEngine;
using UnityEngine.SceneManagement; // Required for Scene Management

public class LevelLoader : MonoBehaviour
{
    // Method to load a scene by its name
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    // Method to load a scene by its index (optional)
    public void LoadLevel(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}

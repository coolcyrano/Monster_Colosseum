using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance;

    // List to store the unlocked units
    public List<GameObject> unlockedUnits = new List<GameObject>();

    private void Awake()
    {
        // Singleton pattern to persist data between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to add a new unit to the unlocked list
    public void UnlockUnit(GameObject unit)
    {
        if (!unlockedUnits.Contains(unit))
        {
            unlockedUnits.Add(unit);
        }
    }
}

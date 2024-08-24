using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameMan : MonoBehaviour
{
    public int maxMoney = 100;
    public int currentMoney;
    public TextMeshProUGUI moneyText;
    public List<GameObject> unitPrefabs; // List of unlocked units
    public List<Transform> spawnPoints; // List of spawn points
    public List<GameObject> possibleUnits; // List of units that can be unlocked
    public List<GameObject> availableUnitsUI; // List of UI elements for unit selection
    public float spawnDelay = 0.5f; // Delay between spawning units, changeable in the Inspector

    private bool isSpawning = false; // To check if a unit is currently being spawned

    private void Start()
    {
        currentMoney = maxMoney;
        UpdateMoneyUI();
        UpdateUnitSelection();
    }

    public void TryPlaceUnit(int unitIndex)
    {
        // Check if the unitIndex corresponds to an unlocked unit
        if (unitIndex < 0 || unitIndex >= unitPrefabs.Count)
        {
            Debug.Log("Unit not unlocked yet or invalid index.");
            return;
        }

        GameObject unitPrefab = unitPrefabs[unitIndex];
        Unit unit = unitPrefab.GetComponent<Unit>();

        if (currentMoney >= unit.cost && !isSpawning)
        {
            StartCoroutine(SpawnUnitWithDelay(unitPrefab));
            currentMoney -= unit.cost;
            UpdateMoneyUI();
        }
        else if (isSpawning)
        {
            Debug.Log("Please wait before placing another unit.");
        }
        else
        {
            Debug.Log("Not enough money to place this unit.");
        }
    }

    private IEnumerator SpawnUnitWithDelay(GameObject unitPrefab)
    {
        isSpawning = true;

        foreach (var spawnPoint in spawnPoints)
        {
            PlaceUnit(unitPrefab, spawnPoint);
            yield return new WaitForSeconds(spawnDelay); // Use the inspector-adjustable delay
        }

        isSpawning = false;
    }

    private void PlaceUnit(GameObject unitPrefab, Transform spawnPoint)
    {
        Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: $" + currentMoney.ToString();
        }
    }

    public void OnLevelComplete()
    {
        Debug.Log("OnLevelComplete() has been called!");

        UnlockRandomUnit();
        UpdateUnitSelection();
        ReturnToLevelSelector();
    }

    private void UnlockRandomUnit()
    {
        if (possibleUnits.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleUnits.Count);
            GameObject randomUnit = possibleUnits[randomIndex];

            if (!unitPrefabs.Contains(randomUnit))
            {
                unitPrefabs.Add(randomUnit);
                Debug.Log("Unlocked unit: " + randomUnit.name);
            }
        }
    }

    private void UpdateUnitSelection()
    {
        foreach (var unitUI in availableUnitsUI)
        {
            GameObject unitPrefab = unitUI.GetComponent<UnitUI>().unitPrefab;

            if (unitPrefabs.Contains(unitPrefab))
            {
                unitUI.SetActive(true);
            }
            else
            {
                unitUI.SetActive(false);
            }
        }
    }

    private void ReturnToLevelSelector()
    {
        SceneManager.LoadScene("level selector"); // Replace "LevelSelector" with your actual scene name
    }
}

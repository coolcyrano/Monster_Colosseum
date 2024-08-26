using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMan : MonoBehaviour
{
    public int maxMoney = 100;
    public int currentMoney;
    public TextMeshProUGUI moneyText;
    public List<GameObject> unitPrefabs; // List of unlocked units
    public List<Transform> spawnPoints; // List of spawn points
    public List<GameObject> possibleUnits; // List of all possible units (unlockable)
    public List<GameObject> availableUnitsUI; // List of UI elements for unit selection
    public float spawnDelay = 0.5f; // Delay between spawning units, changeable in the Inspector
    public LayerMask blueTeamLayer;  // Layer of the enemy team
    public LayerMask redTeamLayer;   // Layer of the ally team

    private bool isSpawning = false; // To check if a unit is currently being spawned

    private void Start()
    {
        currentMoney = maxMoney;
        UpdateMoneyUI();
        UpdateUnitSelection();
    }

    public void TryPlaceUnit(int unitIndex)
    {
        if (unitIndex < 0 || unitIndex >= unitPrefabs.Count)
        {
            Debug.Log("Unit not unlocked yet or invalid index.");
            return;
        }

        GameObject unitPrefab = unitPrefabs[unitIndex];
        Unit unit = unitPrefab.GetComponent<Unit>();

        if (unit != null)
        {
            unit.Initialize(blueTeamLayer, redTeamLayer);  // Initialize with the correct layers
        }

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

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        PlaceUnit(unitPrefab, selectedSpawnPoint);
        yield return new WaitForSeconds(spawnDelay);

        isSpawning = false;
    }

    private void PlaceUnit(GameObject unitPrefab, Transform spawnPoint)
    {
        Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void UpdateUnitSelection()
    {
        for (int i = 0; i < availableUnitsUI.Count; i++)
        {
            if (i < unitPrefabs.Count && unitPrefabs[i] != null)
            {
                availableUnitsUI[i].SetActive(true);
            }
            else
            {
                availableUnitsUI[i].SetActive(false);
            }
        }
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = $"Money: {currentMoney}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

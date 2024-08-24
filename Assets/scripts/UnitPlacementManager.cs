using UnityEngine;

public class UnitPlacementManager : MonoBehaviour
{
    public GameObject[] unitPrefabs;  // Array of unit prefabs
    private GameObject selectedUnitPrefab;  // Currently selected unit prefab

    private void Update()
    {
        // Check if the player clicks the left mouse button
        if (Input.GetMouseButtonDown(0) && selectedUnitPrefab != null)
        {
            PlaceUnit();
        }
    }

    public void SelectUnit(int unitIndex)
    {
        // Set the selected unit prefab based on the button clicked
        selectedUnitPrefab = unitPrefabs[unitIndex];
        Debug.Log($"{selectedUnitPrefab.name} selected for placement.");
    }

    private void PlaceUnit()
    {
        // Raycast to determine where to place the unit
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            // Place the selected unit at the mouse position
            Vector2 placementPosition = hit.point;
            Instantiate(selectedUnitPrefab, placementPosition, Quaternion.identity);
        }
        else
        {
            // If nothing is hit, place the unit at the mouse position on the ground level
            Vector3 placementPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            placementPosition.z = 0;  // Ensure it's on the correct plane
            Instantiate(selectedUnitPrefab, placementPosition, Quaternion.identity);
        }
    }
}

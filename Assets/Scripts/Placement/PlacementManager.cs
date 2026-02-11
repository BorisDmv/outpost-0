using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    [Header("Placeable Objects")]
    public PlaceableObjectData[] placeableObjects;

    private PlaceableObjectData currentData;

    [Header("Settings")]
    public LayerMask groundLayer;
    public float yOffset = 0f; // Adjust this in Inspector so object sits on ground

    private GameObject currentGhost;

    // Call this from your UI Button
    // Call this from your UI Button, passing the index of the object to place
    public void StartPlacement(int objectIndex)
    {
        // Prevent starting placement if already active
        if (currentGhost != null)
        {
            Debug.Log("Placement already active. Cancel or finish before starting a new one.");
            return;
        }
        if (objectIndex < 0 || objectIndex >= placeableObjects.Length)
        {
            Debug.LogError("Invalid placeable object index");
            return;
        }
        currentData = placeableObjects[objectIndex];
        currentGhost = Instantiate(currentData.ghostPrefab);
    }

    void Update()
    {
        if (currentGhost == null) return;

        // Get mouse position from New Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            // Calculate position with the height offset
            Vector3 placementPos = hit.point + new Vector3(0, yOffset, 0);
            currentGhost.transform.position = placementPos;

            // Place on Left Click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (currentGhost.GetComponent<PlacementGhost>().CanPlace())
                {
                    // Check resources before placing
                    if (ResourcesManager.Instance.HasEnoughResources(currentData.cost))
                    {
                        ResourcesManager.Instance.SpendResources(currentData.cost);
                        PlaceObject(placementPos);
                    }
                    else
                    {
                        Debug.Log("Not enough resources to place this object! You need more:");
                        foreach (var cost in currentData.cost)
                        {
                            int have = ResourcesManager.Instance.GetResourceCount(cost.resourceType);
                            if (have < cost.amount)
                                Debug.Log($"- {cost.resourceType}: {have}/{cost.amount}");
                        }
                    }
                }
                else
                {
                    Debug.Log("Cannot place here - obstructed!");
                }
            }
        }

        // Cancel on Right Click
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Destroy(currentGhost);
            currentGhost = null;
            currentData = null;
        }

        // Cancel on Escape key
        //if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        //{
        //    Destroy(currentGhost);
        //    currentGhost = null;
        //    currentData = null;
        //    Debug.Log("Placement cancelled by Escape.");
        //}
    }

    void PlaceObject(Vector3 position)
    {
        if (currentData == null)
        {
            Debug.LogError("No current placeable object data set!");
            return;
        }
        GameObject newBuilding = Instantiate(currentData.actualPrefab, position, Quaternion.identity);

        // Finalize the building (hides radius, sets state)
        if(newBuilding.TryGetComponent(out PlacedObject script))
        {
            script.SetAsPlaced();
        }

        Destroy(currentGhost);
    }
}
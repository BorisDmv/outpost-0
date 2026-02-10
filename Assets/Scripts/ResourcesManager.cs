
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    // For Unity Inspector (since Dictionary is not serializable), use this helper
    [System.Serializable]
    public struct ResourceTextEntry
    {
        public string resourceType;
        public TMP_Text textField;
    }
    public ResourceTextEntry[] resourceTextEntries;

    [System.Serializable]
    public struct ResourceEntry
    {
        public string resourceType;
        public int amount;
    }
    // Exposed in Inspector for manual tweaking
    public ResourceEntry[] resourceEntries;

    // Dictionary to store resource counts by type (internal)
    private Dictionary<string, int> resourceCounts = new Dictionary<string, int>();

    // Assign these in the Inspector: resource type name -> TMP_Text field
    public Dictionary<string, TMP_Text> resourceTextFields = new Dictionary<string, TMP_Text>();
    // Sync resourceCounts and UI when values are changed in the Inspector
    private void OnValidate()
    {
        // Sync resourceCounts from resourceEntries
        if (resourceEntries != null)
        {
            resourceCounts.Clear();
            foreach (var entry in resourceEntries)
            {
                if (!string.IsNullOrEmpty(entry.resourceType))
                {
                    resourceCounts[entry.resourceType] = entry.amount;
                    UpdateResourceUI(entry.resourceType);
                }
            }
        }
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Populate the dictionary from the array for Inspector assignment
        resourceTextFields.Clear();
        foreach (var entry in resourceTextEntries)
        {
            if (!string.IsNullOrEmpty(entry.resourceType) && entry.textField != null)
            {
                resourceTextFields[entry.resourceType] = entry.textField;
            }
        }

        // Sync resourceCounts from resourceEntries for manual tweaking
        resourceCounts.Clear();
        if (resourceEntries != null)
        {
            foreach (var entry in resourceEntries)
            {
                if (!string.IsNullOrEmpty(entry.resourceType))
                {
                    resourceCounts[entry.resourceType] = entry.amount;
                }
            }
            // Update UI for all resources at start
            foreach (var entry in resourceEntries)
            {
                if (!string.IsNullOrEmpty(entry.resourceType))
                {
                    UpdateResourceUI(entry.resourceType);
                }
            }
        }
    }

    // Add resource and update UI
    public void AddResource(string resourceType, int amount)
    {
        if (!resourceCounts.ContainsKey(resourceType))
        {
            resourceCounts[resourceType] = 0;
        }
        resourceCounts[resourceType] += amount;
        // Also update resourceEntries for Inspector visibility
        if (resourceEntries != null)
        {
            for (int i = 0; i < resourceEntries.Length; i++)
            {
                if (resourceEntries[i].resourceType == resourceType)
                {
                    resourceEntries[i].amount = resourceCounts[resourceType];
                }
            }
        }
        UpdateResourceUI(resourceType);
    }


    // Get current resource count
    public int GetResourceCount(string resourceType)
    {
        return resourceCounts.TryGetValue(resourceType, out int count) ? count : 0;
    }

    // Check if enough resources for a given cost array
    public bool HasEnoughResources(PlaceableObjectData.ResourceCost[] costs)
    {
        foreach (var cost in costs)
        {
            if (GetResourceCount(cost.resourceType) < cost.amount)
                return false;
        }
        return true;
    }

    // Spend resources for a given cost array
    public void SpendResources(PlaceableObjectData.ResourceCost[] costs)
    {
        foreach (var cost in costs)
        {
            if (cost.amount > 0)
            {
                AddResource(cost.resourceType, -cost.amount);
            }
        }
    }

    // Update the UI text for the given resource type
    private void UpdateResourceUI(string resourceType)
    {
        if (resourceTextFields.TryGetValue(resourceType, out TMP_Text textField) && textField != null)
        {
            textField.text = $"{resourceType}: {resourceCounts[resourceType]}";
        }
        else
        {
            Debug.Log($"Resource {resourceType} updated: {resourceCounts[resourceType]}");
        }
    }
}

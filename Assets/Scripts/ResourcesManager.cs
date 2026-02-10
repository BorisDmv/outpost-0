
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    // Dictionary to store resource counts by type
    private Dictionary<string, int> resourceCounts = new Dictionary<string, int>();

    // Assign these in the Inspector: resource type name -> TMP_Text field
    public Dictionary<string, TMP_Text> resourceTextFields = new Dictionary<string, TMP_Text>();

    // For Unity Inspector (since Dictionary is not serializable), use this helper
    [System.Serializable]
    public struct ResourceTextEntry
    {
        public string resourceType;
        public TMP_Text textField;
    }
    public ResourceTextEntry[] resourceTextEntries;


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
    }

    // Add resource and update UI
    public void AddResource(string resourceType, int amount)
    {
        if (!resourceCounts.ContainsKey(resourceType))
        {
            resourceCounts[resourceType] = 0;
        }
        resourceCounts[resourceType] += amount;
        UpdateResourceUI(resourceType);
    }

    // Get current resource count
    public int GetResourceCount(string resourceType)
    {
        return resourceCounts.TryGetValue(resourceType, out int count) ? count : 0;
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

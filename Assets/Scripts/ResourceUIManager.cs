using UnityEngine;
using TMPro;

public class ResourceUIManager : MonoBehaviour
{
    public GameObject panel; // Assign in Inspector
    public TMP_Text amountText; // Assign in Inspector

    private static ResourceUIManager _instance;
    public static ResourceUIManager Instance => _instance;

    private Resource selectedResource = null;
    private string selectedResourceType = null;

    public bool IsPanelOpen => panel != null && panel.activeSelf;
    public bool IsResourceSelected(Resource resource) => selectedResource == resource;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (panel != null && panel.activeSelf)
        {
            if (selectedResource == null || selectedResource.gameObject == null)
            {
                HidePanel();
                return;
            }
            amountText.text = $"{selectedResourceType}: {selectedResource.amount}";
            if (selectedResource.amount < 1)
            {
                HidePanel();
            }
        }
    }

    public void ShowResourceAmount(string resourceType, int amount, Resource resource = null)
    {
        if (panel != null && amountText != null)
        {
            panel.SetActive(true);
            amountText.text = $"{resourceType}: {amount}";
            selectedResource = resource;
            selectedResourceType = resourceType;
        }
    }

    public void HidePanel()
    {
        if (panel != null)
            panel.SetActive(false);
        selectedResource = null;
        selectedResourceType = null;
    }
}

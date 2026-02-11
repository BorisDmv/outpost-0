using UnityEngine;
using TMPro;

public class ResourceUIManager : MonoBehaviour
{
    public GameObject panel; // Assign in Inspector
    public TMP_Text amountText; // Assign in Inspector

    private static ResourceUIManager _instance;
    public static ResourceUIManager Instance => _instance;

    private GameObject selectedObject = null;
    private string selectedObjectName = null;

    public bool IsPanelOpen => panel != null && panel.activeSelf;
    public bool IsObjectSelected(GameObject obj) => selectedObject == obj;

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
            if (selectedObject == null)
            {
                HidePanel();
                return;
            }
            UpdateSelectedText();
        }
    }

    public void ShowObjectInfo(GameObject obj)
    {
        if (panel != null && amountText != null)
        {
            panel.SetActive(true);
            selectedObject = obj;
            selectedObjectName = obj != null ? obj.name : null;
            UpdateSelectedText();
        }
    }

    public void HidePanel()
    {
        if (panel != null)
            panel.SetActive(false);
        selectedObject = null;
        selectedObjectName = null;
    }

    private void UpdateSelectedText()
    {
        if (selectedObject == null || amountText == null)
        {
            return;
        }

        var resource = selectedObject.GetComponent<Resource>();
        if (resource != null)
        {
            amountText.text = $"{selectedObjectName}: {resource.amount}";
            if (resource.amount < 1)
            {
                HidePanel();
            }
            return;
        }

        var collector = selectedObject.GetComponent<ResourceCollector>();
        if (collector != null)
        {
            amountText.text = $"Defence Health: {collector.currentHealth}/{collector.maxHealth}";
            if (collector.currentHealth < 1)
            {
                HidePanel();
            }
            return;
        }

        var defenseShooter = selectedObject.GetComponent<DefenseShooter>();
        if (defenseShooter != null)
        {
            amountText.text = $"Collector Health: {defenseShooter.currentHealth}/{defenseShooter.maxHealth}";
            if (defenseShooter.currentHealth < 1)
            {
                HidePanel();
            }
            return;
        }

        var baseObj = selectedObject.GetComponent<Base>();
        if (baseObj != null)
        {
            string progress = "";
            var mgr = ResourcesManager.Instance;
            if (mgr != null && mgr.winConditions != null && mgr.winConditions.Length > 0)
            {
                progress = "\n\nWin Progress:\n";
                foreach (var cond in mgr.winConditions)
                {
                    int current = mgr.GetResourceCount(cond.resourceType);
                    progress += $"{cond.resourceType}: {current}/{cond.amount}\n";
                }
            }
            amountText.text = $"Base Health: {baseObj.currentHealth}/{baseObj.maxHealth}{progress}";
            return;
        }

        amountText.text = selectedObjectName;
    }
}

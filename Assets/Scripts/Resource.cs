using UnityEngine;
public class Resource : MonoBehaviour
{
    public int amount = 10;

    // Called when collected by a ResourceCollector
    public void Collect(int collectAmount)
    {
        amount -= collectAmount;
        if (amount <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Show UI panel with amount when clicked
    void OnMouseDown()
    {
        if (ResourceUIManager.Instance != null)
        {
            ResourceUIManager.Instance.ShowResourceAmount(gameObject.name, amount);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
                var resource = hit.collider.GetComponent<Resource>();
                if (resource != null)
                {
                    ResourceUIManager.Instance?.ShowResourceAmount(hit.collider.gameObject.name, resource.amount);
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing");
            }
        }
    }
}

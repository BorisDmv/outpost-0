using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceClickManager : MonoBehaviour
{
    void Update()
    {
        // Only check if pointer is not over UI
        if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var resource = hit.collider.GetComponent<Resource>();
                if (resource != null)
                {
                    var ui = ResourceUIManager.Instance;
                    if (ui != null)
                    {
                        if (ui.IsPanelOpen && ui.IsResourceSelected(resource))
                        {
                            ui.HidePanel();
                        }
                        else
                        {
                            ui.ShowResourceAmount(hit.collider.gameObject.name, resource.amount, resource);
                        }
                    }
                }
            }
        }
    }
}

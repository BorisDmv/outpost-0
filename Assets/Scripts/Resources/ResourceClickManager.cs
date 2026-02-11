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
                var resource = hit.collider.GetComponentInParent<Resource>();
                var collector = hit.collider.GetComponentInParent<ResourceCollector>();
                var defenseShooter = hit.collider.GetComponentInParent<DefenseShooter>();
                var baseObj = hit.collider.GetComponentInParent<Base>();

                GameObject targetObj = null;
                if (resource != null)
                {
                    targetObj = resource.gameObject;
                }
                else if (collector != null)
                {
                    targetObj = collector.gameObject;
                }
                else if (defenseShooter != null)
                {
                    targetObj = defenseShooter.gameObject;
                }
                else if (baseObj != null)
                {
                    targetObj = baseObj.gameObject;
                }

                if (targetObj != null)
                {
                    var ui = ResourceUIManager.Instance;
                    if (ui != null)
                    {
                        if (ui.IsPanelOpen && ui.IsObjectSelected(targetObj))
                        {
                            ui.HidePanel();
                        }
                        else
                        {
                            ui.ShowObjectInfo(targetObj);
                        }
                    }
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlacedObject : MonoBehaviour
{
    // This allows you to change the radius from other scripts or the inspector
    public float attackRadius
    {
        get => rangeCircle != null ? rangeCircle.radius : 0f;
        set
        {
            if (rangeCircle != null) rangeCircle.UpdateRadius(value);
            UpdateVisualRadius();
        }
    }

    public bool IsPlaced => isPlaced;

    private RangeCircle rangeCircle;
    private bool isPlaced = false;

    void Awake()
    {
        rangeCircle = GetComponentInChildren<RangeCircle>();
    }

    void Start()
    {
        UpdateVisualRadius();
    }

    // Call this if you change the value in the Inspector during Play Mode
    void OnValidate() 
    {
        UpdateVisualRadius();
    }

    public void UpdateVisualRadius()
    {
        // Optionally update visuals if needed
        if (rangeCircle != null)
        {
            rangeCircle.UpdateRadius(rangeCircle.radius);
        }
    }

    public void SetAsPlaced()
    {
        isPlaced = true;
        if (TryGetComponent(out PlacementGhost ghost))
        {
            ghost.SetAsPlaced();
        }
        HideRadius();
    }

    void Update()
    {
        if (isPlaced && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Prevent toggling if pointer is over UI
            if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    ToggleRadius();
                }
            }
        }
    }

    public void ToggleRadius()
    {
        if (rangeCircle != null)
            rangeCircle.gameObject.SetActive(!rangeCircle.gameObject.activeSelf);
    }

    void HideRadius()
    {
        if (rangeCircle != null)
        {
            rangeCircle.gameObject.SetActive(false);
        }
    }
}
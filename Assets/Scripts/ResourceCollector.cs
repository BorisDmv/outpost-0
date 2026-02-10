using UnityEngine;

[RequireComponent(typeof(PlacedObject))]
public class ResourceCollector : MonoBehaviour
{
    [Header("Resource Collection")]
    public string resourceTag = "Resource";
    public float collectionInterval = 2f;
    public int resourceAmountPerTick = 1;

    private float collectionTimer = 0f;
    private RangeCircle rangeCircle;
    private PlacedObject placedObject;

    void Awake()
    {
        rangeCircle = GetComponentInChildren<RangeCircle>();
        placedObject = GetComponent<PlacedObject>();
    }

    void Start()
    {
        collectionTimer = collectionInterval;
    }

    void Update()
    {
        if (placedObject == null || !placedObject.IsPlaced || rangeCircle == null)
        {
            return;
        }

        float radius = rangeCircle.radius;
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(resourceTag))
            {
                collectionTimer -= Time.deltaTime;
                if (collectionTimer <= 0f)
                {
                    CollectResource(hit.gameObject);
                    collectionTimer = collectionInterval;
                }
                break; // Only collect from one resource at a time
            }
        }
    }

    private void CollectResource(GameObject resource)
    {
        // Example: Decrease resource's amount or destroy it (if it has a Resource script)
        var resourceComponent = resource.GetComponent<Resource>();
        if (resourceComponent != null)
        {
            resourceComponent.Collect(resourceAmountPerTick);
        }
        else
        {
            // If no Resource script, just destroy the GameObject as a fallback
            Destroy(resource);
        }

        // Update the resource count in ResourcesManager
        ResourcesManager.Instance.AddResource(resourceTag, resourceAmountPerTick);

        Debug.Log($"Collected {resourceAmountPerTick} from {resource.name}");
    }

    void OnDrawGizmosSelected()
    {
        if (rangeCircle == null)
        {
            rangeCircle = GetComponentInChildren<RangeCircle>();
        }

        if (rangeCircle != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, rangeCircle.radius);
        }
    }
}

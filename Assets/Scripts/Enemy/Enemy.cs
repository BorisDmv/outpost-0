using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth = 50;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float targetRefreshInterval = 0.5f;
    public float attackRange = 0.6f;

    [Header("Attack")]
    public int damageToBuilding = 10;
    public float attackInterval = 1.0f;
    public string placementLayerName = "Placement";

    private Transform currentTarget;
    private float targetRefreshTimer = 0f;
    private int placementLayer = -1;
    private float attackTimer = 0f;

    void Awake()
    {
        placementLayer = LayerMask.NameToLayer(placementLayerName);
        if (placementLayer < 0)
        {
            Debug.LogWarning($"Enemy: Layer '{placementLayerName}' not found. Check Project Settings > Tags and Layers.");
        }
    }

    void Update()
    {
        targetRefreshTimer -= Time.deltaTime;
        if (currentTarget == null || targetRefreshTimer <= 0f)
        {
            currentTarget = FindClosestPlacedTarget();
            targetRefreshTimer = targetRefreshInterval;
        }

        if (currentTarget != null)
        {
            Vector3 targetPos = currentTarget.position;
            Vector3 moveDir = (targetPos - transform.position);
            float distance = moveDir.magnitude;
            if (distance <= attackRange)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    ApplyDamageToTarget(currentTarget.gameObject);
                    attackTimer = attackInterval;
                }
                return;
            }

            Vector3 step = moveDir.normalized * moveSpeed * Time.deltaTime;
            transform.position += step;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private Transform FindClosestPlacedTarget()
    {
        PlacedObject[] placedObjects = FindObjectsOfType<PlacedObject>();
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var placed in placedObjects)
        {
            if (!placed.IsPlaced)
            {
                continue;
            }

            GameObject obj = placed.gameObject;
            if (placementLayer >= 0 && obj.layer != placementLayer)
            {
                continue;
            }

            float dist = (obj.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj.transform;
            }
        }

        return closest;
    }

    private void ApplyDamageToTarget(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (target.TryGetComponent(out Base baseBuilding))
        {
            baseBuilding.TakeDamage(damageToBuilding);
            if (baseBuilding.currentHealth <= 0)
            {
                currentTarget = null;
                attackTimer = 0f;
            }
            return;
        }

        if (target.TryGetComponent(out ResourceCollector collector))
        {
            collector.TakeDamage(damageToBuilding);
            if (collector.currentHealth <= 0)
            {
                currentTarget = null;
                attackTimer = 0f;
            }
            return;
        }

        if (target.TryGetComponent(out DefenseShooter defenseShooter))
        {
            defenseShooter.TakeDamage(damageToBuilding);
            if (defenseShooter.currentHealth <= 0)
            {
                currentTarget = null;
                attackTimer = 0f;
            }
            return;
        }
    }
}

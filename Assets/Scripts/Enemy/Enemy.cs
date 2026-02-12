using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth = 50;

    [Header("Movement")]
    public float moveSpeed = 3f;
    [Header("Rotation")]
    public float rotationSpeed = 180f; // degrees per second
    public float targetRefreshInterval = 0.5f;
    public float attackRange = 0.6f;

    [Header("Attack")]
    public int damageToBuilding = 10;
    public float attackInterval = 1.0f;
    public string placementLayerName = "Placement";
    public bool requirePlacementLayer = false;

    [Header("Death")]
    public ParticleSystem deathExplosionPrefab;
    public float destroyDelay = 0.2f;

    private bool isDying = false;

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
            if (distance > 0.01f)
            {
                // Smoothly rotate towards movement direction
                Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
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

            // Always move directly toward the target
            Vector3 step = moveDir.normalized * moveSpeed * Time.deltaTime;
            transform.position += step;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        if (deathExplosionPrefab != null)
        {
            ParticleSystem ps = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        Destroy(gameObject, destroyDelay);
    }

    private Transform FindClosestPlacedTarget()
    {
        Transform closest = null;
        float minDist = float.MaxValue;

        // 1) Target placed buildings
        PlacedObject[] placedObjects = FindObjectsOfType<PlacedObject>();
        foreach (var placed in placedObjects)
        {
            if (!placed.IsPlaced)
                continue;

            GameObject obj = placed.gameObject;
            if (requirePlacementLayer && placementLayer >= 0 && obj.layer != placementLayer)
                continue;

            float dist = (obj.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj.transform;
            }
        }

        // 2) Also target the Control Tower / Base (doesn't implement PlacedObject)
        Base[] bases = FindObjectsOfType<Base>();
        foreach (var b in bases)
        {
            if (b == null) continue;
            GameObject obj = b.gameObject;
            if (requirePlacementLayer && placementLayer >= 0 && obj.layer != placementLayer)
                continue;

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

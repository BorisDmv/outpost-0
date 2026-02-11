using UnityEngine;

// ...existing code...
[RequireComponent(typeof(PlacedObject))]
public class DefenseShooter : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    [Header("Defense Shooting")]
    public GameObject projectilePrefab;
    public string enemyTag = "Enemy";
    public float attackInterval = 1.0f;
    public float projectileSpeed = 10f;

    [Tooltip("Optional: Where projectiles spawn from. If not set, will use building position.")]
    public Transform projectileSpawnPoint;

    [Header("Visuals")]
    [Tooltip("Optional: The head transform to rotate toward the target.")]
    public Transform headTransform;

    private float attackTimer = 0f;
    private RangeCircle rangeCircle;
    private PlacedObject placedObject;

    // For smooth head rotation
    private Transform currentTarget;
    [Header("Head Rotation")]
    public float headRotationSpeed = 5f;

    void Awake()
    {
        rangeCircle = GetComponentInChildren<RangeCircle>();
        placedObject = GetComponent<PlacedObject>();
    }

    void Start()
    {
        attackTimer = attackInterval;
    }

    void Update()
    {
        if (placedObject == null || !placedObject.IsPlaced || rangeCircle == null)
        {
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            if (projectilePrefab != null)
            {
                GameObject target = FindNearestEnemy();
                if (target != null)
                {
                    currentTarget = target.transform;
                    ShootAtEnemy(target);
                    attackTimer = attackInterval;
                }
            }
        }

        // Smoothly rotate head toward target
        if (headTransform != null && currentTarget != null)
        {
            Vector3 lookDir = (currentTarget.position - headTransform.position).normalized;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                headTransform.rotation = Quaternion.Slerp(headTransform.rotation, targetRot, headRotationSpeed * Time.deltaTime);
            }
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


    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    private GameObject FindNearestEnemy()
    {
        float radius = rangeCircle.radius;
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                float dist = (hit.transform.position - transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.gameObject;
                }
            }
        }
        return nearest;
    }

    private void ShootAtEnemy(GameObject enemy)
    {
        // No longer rotate head instantly; handled in Update for smoothness
        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;
        Vector3 dir = (enemy.transform.position - spawnPos).normalized;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));

        Collider shooterCol = GetComponent<Collider>();
        Collider projCol = proj.GetComponent<Collider>();
        if (shooterCol != null && projCol != null)
        {
            Physics.IgnoreCollision(projCol, shooterCol);
        }

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
            rb.useGravity = false;
        }
    }
}

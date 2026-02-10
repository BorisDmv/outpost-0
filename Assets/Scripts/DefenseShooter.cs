using UnityEngine;

[RequireComponent(typeof(PlacedObject))]
public class DefenseShooter : MonoBehaviour
{
    [Header("Defense Shooting")]
    public GameObject projectilePrefab;
    public string enemyTag = "Enemy";
    public float attackInterval = 1.0f;
    public float projectileSpeed = 10f;

    [Tooltip("Optional: Where projectiles spawn from. If not set, will use building position.")]
    public Transform projectileSpawnPoint;

    private float attackTimer = 0f;
    private RangeCircle rangeCircle;
    private PlacedObject placedObject;

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
                    ShootAtEnemy(target);
                    attackTimer = attackInterval;
                }
            }
        }
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
        }
    }
}

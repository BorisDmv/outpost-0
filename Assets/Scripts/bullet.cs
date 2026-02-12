using UnityEngine;

public class bullet : MonoBehaviour
{
    public float lifetime = 120f; // Bullet will be destroyed after 2 minutes by default
    public int damage = 10;

    private void Awake()
    {
        // Trigger-hit bullets should not physically collide/push.
        // Ensure we have a Rigidbody so Unity can generate trigger events reliably.
        if (!TryGetComponent(out Rigidbody rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = false;

        // Ensure there's at least one trigger collider on this bullet.
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Optional: add bullet movement logic here
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}

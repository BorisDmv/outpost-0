using UnityEngine;

public class bullet : MonoBehaviour
{
    public float lifetime = 120f; // Bullet will be destroyed after 2 minutes by default

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Optional: add bullet movement logic here
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}

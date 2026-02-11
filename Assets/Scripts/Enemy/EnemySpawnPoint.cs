using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float initialDelay = 2f;
    public float spawnInterval = 5f;
    public int maxAliveFromThisSpawner = 0;

    private float spawnTimer = 0f;
    private int aliveCount = 0;

    void Start()
    {
        spawnTimer = initialDelay;
    }

    void Update()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        if (maxAliveFromThisSpawner > 0 && aliveCount >= maxAliveFromThisSpawner)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        aliveCount++;
        var tracker = enemy.AddComponent<SpawnedEnemyTracker>();
        tracker.Init(this);
    }

    private void OnEnemyDestroyed()
    {
        if (aliveCount > 0)
        {
            aliveCount--;
        }
    }

    private class SpawnedEnemyTracker : MonoBehaviour
    {
        private EnemySpawnPoint owner;

        public void Init(EnemySpawnPoint spawner)
        {
            owner = spawner;
        }

        void OnDestroy()
        {
            if (owner != null)
            {
                owner.OnEnemyDestroyed();
            }
        }
    }
}

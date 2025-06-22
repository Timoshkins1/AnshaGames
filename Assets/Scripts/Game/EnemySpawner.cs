using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float minSpawnDistance = 10f;
    [SerializeField] private int initialEnemies = 3;
    [SerializeField] private int enemiesIncreasePerWave = 2;
    [SerializeField] private int maxEnemiesPerWave = 10;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnHeightCheck = 2f;
    [SerializeField] private float spawnCheckRadius = 1f;
    [SerializeField] private bool debugLogging = false;

    [Header("Timing Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private float periodicSpawnInterval = 8f;

    [Header("Independent Spawn Settings")]
    [SerializeField] private float independentSpawnInterval = 60f;
    [SerializeField] private int independentSpawnCount = 5;
    [SerializeField] private float independentSpawnRadius = 20f;

    [Header("Enemy Scaling")]
    [SerializeField] private float healthIncreasePerWave = 0.1f;
    [SerializeField] private float walkSpeedIncreasePerWave = 0.05f;
    [SerializeField] private float runSpeedIncreasePerWave = 0.05f;
    [SerializeField] private float damageIncreasePerWave = 0.1f;
    [SerializeField] private float runDetectionRangeIncreasePerWave = 0.05f;

    [Header("Spawn Points Pool")]
    [SerializeField] private Transform spawnPointsContainer;
    [SerializeField] private float minDistanceToSpawnPoint = 15f;
    [SerializeField] private bool useSpawnPointsPool = true;

    private Transform player;
    private int enemiesToSpawn;
    private int currentWave;
    private float spawnTimer;
    private float periodicSpawnTimer;
    private float independentSpawnTimer;
    private NavMeshHit navHit;
    private int spawnAttemptCount = 0;
    private Vector3[] precomputedDirections = new Vector3[16];
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentMaxEnemies;
    private List<Transform> spawnPoints = new List<Transform>();

    public static EnemySpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PrecomputeDirections();
        InitializeSpawnPoints();
    }

    private void InitializeSpawnPoints()
    {
        if (spawnPointsContainer != null && useSpawnPointsPool)
        {
            foreach (Transform child in spawnPointsContainer)
            {
                spawnPoints.Add(child);
            }
            if (debugLogging) Debug.Log($"Initialized {spawnPoints.Count} spawn points");
        }
    }

    public void SetPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
        if (debugLogging) Debug.Log("Player reference set in EnemySpawner");
    }

    private void Start()
    {
        if (debugLogging) Debug.Log("EnemySpawner initialized");

        if (PlayerManager.Instance != null)
        {
            SetPlayerTransform(PlayerManager.Instance.PlayerTransform);
        }

        periodicSpawnTimer = periodicSpawnInterval;
        independentSpawnTimer = independentSpawnInterval;
        currentMaxEnemies = initialEnemies;
    }

    private void PrecomputeDirections()
    {
        for (int i = 0; i < precomputedDirections.Length; i++)
        {
            float angle = i * Mathf.PI * 2f / precomputedDirections.Length;
            precomputedDirections[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        }
    }

    public void StartWave(int wave)
    {
        currentWave = wave;
        currentMaxEnemies = initialEnemies + (wave - 1) * enemiesIncreasePerWave;

        if (maxEnemiesPerWave > 0 && currentMaxEnemies > maxEnemiesPerWave)
        {
            currentMaxEnemies = maxEnemiesPerWave;
        }

        int enemiesToSpawnNow = Mathf.Min(3 + (wave - 1) * 2, currentMaxEnemies - activeEnemies.Count);
        enemiesToSpawn = enemiesToSpawnNow;

        spawnTimer = 0f;
        if (debugLogging) Debug.Log($"Starting wave {wave}. Max enemies: {currentMaxEnemies}");
    }

    private void Update()
    {
        if (enemiesToSpawn > 0)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                TrySpawnEnemy();
                enemiesToSpawn--;
            }
        }

        periodicSpawnTimer += Time.deltaTime;
        if (periodicSpawnTimer >= periodicSpawnInterval && activeEnemies.Count < currentMaxEnemies)
        {
            periodicSpawnTimer = 0f;
            SpawnPeriodicEnemies();
        }

        independentSpawnTimer -= Time.deltaTime;
        if (independentSpawnTimer <= 0f)
        {
            independentSpawnTimer = independentSpawnInterval;
            SpawnIndependentEnemies();
        }
    }

    private void SpawnIndependentEnemies()
    {
        for (int i = 0; i < independentSpawnCount; i++)
        {
            TrySpawnEnemyIndependent();
        }
        if (debugLogging) Debug.Log($"Independent spawn: {independentSpawnCount} enemies");
    }

    private void TrySpawnEnemyIndependent()
    {
        if (!enemyPrefab || !player) return;

        // Используем ту же логику выбора точек, что и в основном спавне
        if (TryStandardSpawn())
        {
            return;
        }

        if (useSpawnPointsPool && spawnPoints.Count > 0)
        {
            TryFallbackSpawnFromPool();
        }
    }

    private void SpawnPeriodicEnemies()
    {
        int availableSlots = currentMaxEnemies - activeEnemies.Count;
        if (availableSlots <= 0) return;

        int maxSpawn = Mathf.Min(3 + currentWave / 3, availableSlots);
        int enemiesToSpawn = Random.Range(1, maxSpawn + 1);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            TrySpawnEnemy();
        }
        if (debugLogging) Debug.Log($"Periodic spawn: {enemiesToSpawn} enemies");
    }

    private void TrySpawnEnemy()
    {
        if (!enemyPrefab || !player || activeEnemies.Count >= currentMaxEnemies) return;

        if (TryStandardSpawn())
        {
            return;
        }

        if (useSpawnPointsPool && spawnPoints.Count > 0)
        {
            TryFallbackSpawnFromPool();
        }
    }

    private bool TryStandardSpawn()
    {
        spawnAttemptCount = 0;

        for (int i = 0; i < precomputedDirections.Length && spawnAttemptCount < maxSpawnAttempts; i++)
        {
            Vector3 dir = (precomputedDirections[i] + Random.insideUnitSphere * 0.3f).normalized;
            if (TrySpawnAtDirection(dir))
                return true;
        }

        while (spawnAttemptCount < maxSpawnAttempts)
        {
            Vector3 spawnDirection = Random.onUnitSphere;
            spawnDirection.y = 0;
            spawnDirection.Normalize();

            if (TrySpawnAtDirection(spawnDirection))
                return true;
        }

        return false;
    }

    private void TryFallbackSpawnFromPool()
    {
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        while (availablePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randomIndex];
            availablePoints.RemoveAt(randomIndex);

            if (IsValidSpawnPosition(spawnPoint.position, out _))
            {
                SpawnEnemy(spawnPoint.position);
                if (debugLogging) Debug.Log($"Enemy spawned from fallback pool at {spawnPoint.position}");
                return;
            }
        }

        if (debugLogging) Debug.LogWarning("Failed to spawn using fallback spawn points");
    }

    private bool TrySpawnAtDirection(Vector3 direction)
    {
        spawnAttemptCount++;

        float distance = spawnRadius * (0.8f + Random.Range(0f, 0.4f));
        Vector3 spawnPosition = player.position + direction * distance;

        if (Vector3.Distance(spawnPosition, player.position) < minSpawnDistance)
        {
            spawnPosition = player.position + direction * minSpawnDistance;
        }

        if (IsValidSpawnPosition(spawnPosition, out _))
        {
            SpawnEnemy(spawnPosition);
            return true;
        }
        return false;
    }

    private void SpawnEnemy(Vector3 position)
    {
        var enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        ApplyWaveStats(enemy, currentWave);
        activeEnemies.Add(enemy);

        var health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.OnDeath += () => RemoveEnemy(enemy);
        }

        if (debugLogging) Debug.Log($"Enemy spawned at {position}. Total enemies: {activeEnemies.Count}");
    }

    private void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            if (debugLogging) Debug.Log($"Enemy removed. Total enemies: {activeEnemies.Count}");
        }
    }

    private void ApplyWaveStats(GameObject enemy, int wave)
    {
        if (wave <= 1) return;

        var health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.SetMaxHealth(Mathf.RoundToInt(health.MaxHealth * (1 + healthIncreasePerWave * (wave - 1))));
        }

        var ai = enemy.GetComponent<BrawlStarsBotAI>();
        if (ai != null)
        {
            ai.MovementSpeed *= (1 + walkSpeedIncreasePerWave * (wave - 1));
            ai.RunningSpeed *= (1 + runSpeedIncreasePerWave * (wave - 1));
            ai.damage = Mathf.RoundToInt(ai.damage * (1 + damageIncreasePerWave * (wave - 1)));
            ai.runDetectionRange *= (1 + runDetectionRangeIncreasePerWave * (wave - 1));
        }
    }

    private bool IsValidSpawnPosition(Vector3 position, out string failReason)
    {
        if (!Physics.Raycast(position + Vector3.up * spawnHeightCheck, Vector3.down,
                           spawnHeightCheck * 2, groundLayer))
        {
            failReason = "No ground";
            return false;
        }

        Collider[] colliders = Physics.OverlapSphere(position, spawnCheckRadius, obstacleLayer);
        if (colliders.Length > 0)
        {
            failReason = "Obstacle";
            return false;
        }

        if (!NavMesh.SamplePosition(position, out navHit, spawnCheckRadius, NavMesh.AllAreas))
        {
            failReason = "No NavMesh";
            return false;
        }

        if (Vector3.Distance(player.position, position) < minSpawnDistance)
        {
            failReason = "Too close";
            return false;
        }

        failReason = "Valid";
        return true;
    }

    public void CleanupAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        activeEnemies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(player.position, minSpawnDistance);
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(player.position, spawnRadius);

            if (precomputedDirections != null && precomputedDirections.Length > 0)
            {
                Gizmos.color = Color.cyan;
                foreach (var dir in precomputedDirections)
                {
                    Vector3 spawnPos = player.position + dir * spawnRadius;
                    Gizmos.DrawLine(player.position, spawnPos);
                    Gizmos.DrawSphere(spawnPos, 0.5f);
                }
            }
        }

        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.7f);
                    Gizmos.DrawWireSphere(point.position, minDistanceToSpawnPoint);
                }
            }
        }
    }
}
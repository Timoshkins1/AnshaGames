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
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnHeightCheck = 2f;
    [SerializeField] private float spawnCheckRadius = 1f;
    [SerializeField] private bool debugLogging = false;

    [Header("Timing Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float waveCooldown = 5f;
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("Enemy Scaling")]
    [SerializeField] private float healthIncreasePerWave = 0.1f;
    [SerializeField] private float walkSpeedIncreasePerWave = 0.05f;
    [SerializeField] private float runSpeedIncreasePerWave = 0.05f;
    [SerializeField] private float damageIncreasePerWave = 0.1f;
    [SerializeField] private float runDetectionRangeIncreasePerWave = 0.05f;

    private Transform player;
    private int enemiesToSpawn;
    private int currentWave;
    private float spawnTimer;
    private NavMeshHit navHit;
    private int spawnAttemptCount = 0;
    private Vector3[] precomputedDirections = new Vector3[8]; // Предварительно вычисленные направления
    private List<GameObject> activeEnemies = new List<GameObject>();

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
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (debugLogging) Debug.Log("EnemySpawner initialized");
    }

    private void PrecomputeDirections()
    {
        // Предварительно вычисляем 8 основных направлений для спавна
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8;
            precomputedDirections[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }
    }

    public void StartWave(int wave)
    {
        currentWave = wave;
        enemiesToSpawn = initialEnemies + (wave - 1) * enemiesIncreasePerWave;
        spawnTimer = spawnInterval; // Начинаем спавн сразу
        if (debugLogging) Debug.Log($"Starting wave {wave}");
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
    }

    private void TrySpawnEnemy()
    {
        if (!enemyPrefab || !player) return;

        // Пробуем предварительно вычисленные направления сначала
        for (int i = 0; i < precomputedDirections.Length && spawnAttemptCount < maxSpawnAttempts; i++)
        {
            if (TrySpawnAtDirection(precomputedDirections[i]))
                return;
        }

        // Если не получилось, пробуем случайные направления
        while (spawnAttemptCount < maxSpawnAttempts)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 spawnDirection = new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (TrySpawnAtDirection(spawnDirection))
                return;
        }

        if (debugLogging) Debug.LogWarning($"Failed to spawn after {maxSpawnAttempts} attempts");
    }

    private bool TrySpawnAtDirection(Vector3 direction)
    {
        spawnAttemptCount++;
        Vector3 spawnPosition = player.position + direction * (spawnRadius + Random.Range(0f, 5f));

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
        if (debugLogging) Debug.Log($"Enemy spawned at {position}");
    }

    private void ApplyWaveStats(GameObject enemy, int wave)
    {
        if (wave <= 1) return;

        float waveMultiplier = 1 + (wave - 1) * 0.1f; // Общий множитель для оптимизации вычислений

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
        // Проверка земли
        if (!Physics.Raycast(position + Vector3.up * spawnHeightCheck, Vector3.down,
                           spawnHeightCheck * 2, groundLayer))
        {
            failReason = "No ground";
            return false;
        }

        // Проверка препятствий
        if (Physics.CheckSphere(position, spawnCheckRadius, obstacleLayer))
        {
            failReason = "Obstacle";
            return false;
        }

        // Проверка NavMesh
        if (!NavMesh.SamplePosition(position, out navHit, spawnCheckRadius, NavMesh.AllAreas))
        {
            failReason = "No NavMesh";
            return false;
        }

        // Проверка расстояния
        if ((player.position - position).sqrMagnitude < minSpawnDistance * minSpawnDistance)
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
        }
    }
}
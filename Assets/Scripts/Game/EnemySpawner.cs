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
    [SerializeField] private float waveCooldown = 5f;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private float periodicSpawnInterval = 8f;

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
    private float periodicSpawnTimer;
    private NavMeshHit navHit;
    private int spawnAttemptCount = 0;
    private Vector3[] precomputedDirections = new Vector3[16]; // Увеличили до 16 направлений
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentMaxEnemies;

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
        currentMaxEnemies = initialEnemies;
    }

    private void PrecomputeDirections()
    {
        // Увеличили количество направлений до 16 для более равномерного распределения
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

        // Спавним больше врагов в начале волны
        int enemiesToSpawnNow = Mathf.Min(3 + (wave - 1) * 2, currentMaxEnemies - activeEnemies.Count);
        enemiesToSpawn = enemiesToSpawnNow;

        spawnTimer = 0f; // Начинаем спавн сразу
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
    }

    private void SpawnPeriodicEnemies()
    {
        int availableSlots = currentMaxEnemies - activeEnemies.Count;
        if (availableSlots <= 0) return;

        // Увеличиваем шанс спавна больше врагов на высоких волнах
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

        spawnAttemptCount = 0;

        // Сначала пробуем предварительно вычисленные направления
        for (int i = 0; i < precomputedDirections.Length && spawnAttemptCount < maxSpawnAttempts; i++)
        {
            // Добавляем небольшую случайность к направлению
            Vector3 dir = (precomputedDirections[i] + Random.insideUnitSphere * 0.3f).normalized;
            if (TrySpawnAtDirection(dir))
                return;
        }

        // Затем пробуем полностью случайные направления
        while (spawnAttemptCount < maxSpawnAttempts)
        {
            Vector3 spawnDirection = Random.onUnitSphere;
            spawnDirection.y = 0; // Обнуляем Y для 2D плоскости
            spawnDirection.Normalize();

            if (TrySpawnAtDirection(spawnDirection))
                return;
        }

        if (debugLogging) Debug.LogWarning($"Failed to spawn after {maxSpawnAttempts} attempts");
    }

    private bool TrySpawnAtDirection(Vector3 direction)
    {
        spawnAttemptCount++;

        // Более интеллектуальное вычисление позиции спавна
        float distance = spawnRadius * (0.8f + Random.Range(0f, 0.4f)); // Вариация расстояния
        Vector3 spawnPosition = player.position + direction * distance;

        // Корректировка минимальной дистанции
        if (Vector3.Distance(spawnPosition, player.position) < minSpawnDistance)
        {
            spawnPosition = player.position + direction * minSpawnDistance;
        }

        // Проверка позиции с более умными параметрами
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

        float waveMultiplier = 1 + (wave - 1) * 0.1f;

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
        // Улучшенная проверка позиции
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

            // Рисуем направления спавна для наглядности
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
    }
}
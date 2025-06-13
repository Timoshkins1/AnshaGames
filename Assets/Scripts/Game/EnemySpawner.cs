using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float minSpawnDistance = 10f;
    [SerializeField] private int initialEnemies = 3;
    [SerializeField] private int enemiesIncreasePerWave = 2;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float spawnHeightCheck = 2f;
    [SerializeField] private float spawnCheckRadius = 1f;
    [SerializeField] private bool debugLogging = true;

    [Header("Настройки интервалов")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float waveCooldown = 5f;
    [SerializeField] private int maxSpawnAttempts = 10;

    private Transform player;
    private int enemiesToSpawn;
    private int currentWave;
    private float spawnTimer;
    private NavMeshHit navHit;
    private int spawnAttemptCount = 0;

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
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (debugLogging) Debug.Log("EnemySpawner initialized. Player: " + player.name);
    }

    public void StartWave(int wave)
    {
        currentWave = wave;
        enemiesToSpawn = initialEnemies + (wave - 1) * enemiesIncreasePerWave;
        spawnTimer = 0f;
        if (debugLogging) Debug.Log($"Starting wave {wave}. Enemies to spawn: {enemiesToSpawn}");
    }

    private void Update()
    {
        if (enemiesToSpawn > 0)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                spawnAttemptCount = 0;
                TrySpawnEnemy();
                enemiesToSpawn--;
            }
        }
    }

    private void TrySpawnEnemy()
    {
        if (!enemyPrefab)
        {
            if (debugLogging) Debug.LogError("Enemy prefab is not assigned!");
            return;
        }

        if (!player)
        {
            if (debugLogging) Debug.LogError("Player reference is missing!");
            return;
        }

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            spawnAttemptCount++;
            Vector3 spawnPosition = GetRandomSpawnPosition();

            if (IsValidSpawnPosition(spawnPosition, out string failReason))
            {
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                if (debugLogging) Debug.Log($"Enemy spawned at {spawnPosition} (attempt {spawnAttemptCount})");
                return;
            }
            else
            {
                if (debugLogging) Debug.Log($"Spawn failed at {spawnPosition}: {failReason}");
            }
        }

        if (debugLogging) Debug.LogWarning($"Failed to spawn enemy after {maxSpawnAttempts} attempts");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        Vector3 spawnDirection = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 spawnPosition = player.position + spawnDirection * (spawnRadius + Random.Range(0f, 5f));

        // Проверяем, чтобы точка спавна не была слишком близко к игроку
        if (Vector3.Distance(spawnPosition, player.position) < minSpawnDistance)
        {
            spawnPosition = player.position + (spawnPosition - player.position).normalized * minSpawnDistance;
        }

        return spawnPosition;
    }

    private bool IsValidSpawnPosition(Vector3 position, out string failReason)
    {
        // Проверка на наличие земли под позицией
        if (!Physics.Raycast(position + Vector3.up * spawnHeightCheck, Vector3.down,
                           spawnHeightCheck * 2, groundLayer))
        {
            failReason = "No ground below";
            return false;
        }

        // Проверка на наличие препятствий
        if (Physics.CheckSphere(position, spawnCheckRadius, obstacleLayer))
        {
            failReason = "Obstacle detected";
            return false;
        }

        // Проверка на доступность в NavMesh
        if (!NavMesh.SamplePosition(position, out navHit, spawnCheckRadius, NavMesh.AllAreas))
        {
            failReason = "Not on NavMesh";
            return false;
        }

        // Фиксируем Y-координату по NavMesh
        position.y = navHit.position.y;

        // Проверка расстояния до игрока
        if (Vector3.Distance(position, player.position) < minSpawnDistance)
        {
            failReason = "Too close to player";
            return false;
        }

        failReason = "Valid position";
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minSpawnDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }

    private void OnGUI()
    {
        if (debugLogging)
        {
            GUI.Label(new Rect(10, 100, 300, 20), $"Current Wave: {currentWave}");
            GUI.Label(new Rect(10, 120, 300, 20), $"Enemies to Spawn: {enemiesToSpawn}");
            GUI.Label(new Rect(10, 140, 300, 20), $"Last Attempts: {spawnAttemptCount}/{maxSpawnAttempts}");
        }
    }
}
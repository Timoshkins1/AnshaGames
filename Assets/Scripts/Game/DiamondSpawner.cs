using UnityEngine;
using System.Collections.Generic;
using System;

public class DiamondSpawner : MonoBehaviour
{

    public event Action<Diamond> OnDiamondSpawned;
    [Header("Spawn Settings")]
    [SerializeField] private GameObject diamondPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("Pooling Settings")]
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private bool expandPool = true;

    private List<Diamond> diamondPool = new List<Diamond>();
    private List<Vector3> precomputedOffsets = new List<Vector3>();


    // Добавьте свойство для доступа к пулу
    public List<Diamond> DiamondPool => diamondPool;
    private const int PrecomputedOffsetCount = 16;

    public static DiamondSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
            PrecomputeOffsets();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePooledDiamond();
        }
    }

    private void CreatePooledDiamond()
    {
        GameObject diamondObj = Instantiate(diamondPrefab);
        Diamond diamond = diamondObj.GetComponent<Diamond>();

        if (diamond == null)
        {
            diamond = diamondObj.AddComponent<Diamond>();
        }

        diamondObj.SetActive(false);
        diamondPool.Add(diamond);
    }

    private void PrecomputeOffsets()
    {
        for (int i = 0; i < PrecomputedOffsetCount; i++)
        {
            float angle = i * (2f * Mathf.PI / PrecomputedOffsetCount);
            precomputedOffsets.Add(new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius);
        }
    }

    public void SpawnWave(int diamondsCount)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        CleanupExistingDiamonds(); // Очистка перед новой волной
        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = diamondsCount * 2;

        while (spawnedCount < diamondsCount && attempts < maxAttempts)
        {
            if (TrySpawnDiamond())
            {
                spawnedCount++;
            }
            attempts++;
        }

        if (spawnedCount < diamondsCount)
        {
            Debug.LogWarning($"Spawned only {spawnedCount} of {diamondsCount} diamonds");
        }
    }

    private void CleanupExistingDiamonds()
    {
        // Деактивируем все алмазы из пула перед новой волной
        foreach (Diamond diamond in diamondPool)
        {
            if (diamond.gameObject.activeInHierarchy)
            {
                diamond.gameObject.SetActive(false);
                diamond.ResetDiamond();
            }
        }
    }

    private bool TrySpawnDiamond()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        // Сначала пробуем предварительно вычисленные позиции
        foreach (Vector3 offset in precomputedOffsets)
        {
            Vector3 spawnPosition = spawnPoint.position + offset;
            if (IsValidSpawnPosition(spawnPosition))
            {
                SpawnAtPosition(spawnPosition);
                return true;
            }
        }

        // Если не получилось, пробуем случайные позиции
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            if (IsValidSpawnPosition(spawnPosition))
            {
                SpawnAtPosition(spawnPosition);
                return true;
            }
        }

        return false;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        return !Physics.CheckSphere(position, checkRadius, obstacleMask);
    }

    // Модифицируйте метод SpawnAtPosition
    private void SpawnAtPosition(Vector3 position)
    {
        Diamond diamond = GetPooledDiamond();
        if (diamond == null)
        {
            if (expandPool)
            {
                CreatePooledDiamond();
                diamond = GetPooledDiamond();
            }
            else
            {
                Debug.LogWarning("Diamond pool exhausted!");
                return;
            }
        }

        diamond.transform.position = position;
        diamond.gameObject.SetActive(true);
        diamond.ResetDiamond();

        // Уведомляем о новом алмазе
        OnDiamondSpawned?.Invoke(diamond);
    }

    private Diamond GetPooledDiamond()
    {
        foreach (Diamond diamond in diamondPool)
        {
            if (!diamond.gameObject.activeInHierarchy)
            {
                return diamond;
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, spawnRadius);
                }
            }
        }
    }
}
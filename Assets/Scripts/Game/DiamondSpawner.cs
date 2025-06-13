using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject diamondPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnRadius = 5f;

    public static DiamondSpawner Instance { get; private set; }

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

    public void SpawnWave(int diamondsCount)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Не указаны точки спавна!");
            return;
        }

        for (int i = 0; i < diamondsCount; i++)
        {
            SpawnDiamond();
        }
    }

    private void SpawnDiamond()
    {
        if (!diamondPrefab) return;

        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0;

        Instantiate(diamondPrefab, randomPoint.position + randomOffset, Quaternion.identity);
    }
}
using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject diamondPrefab; // Префаб алмаза
    [SerializeField] private Transform[] spawnPoints;   // Точки, вокруг которых будет спавн
    [SerializeField] private float spawnRadius = 5f;    // Радиус случайного смещения
    [SerializeField] private float spawnInterval = 10f; // Интервал спавна (в секундах)

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Не указаны точки спавна!");
            return;
        }

        InvokeRepeating(nameof(SpawnDiamond), spawnInterval, spawnInterval);
    }

    private void SpawnDiamond()
    {
        if (!diamondPrefab) return;

        // Выбираем случайную точку спавна
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Получаем случайное смещение в радиусе
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0; // чтобы алмаз не уходил вверх/вниз

        // Спавним алмаз
        Instantiate(diamondPrefab, randomPoint.position + randomOffset, Quaternion.identity);
    }
}
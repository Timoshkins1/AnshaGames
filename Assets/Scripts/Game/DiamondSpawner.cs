using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private GameObject diamondPrefab; // ������ ������
    [SerializeField] private Transform[] spawnPoints;   // �����, ������ ������� ����� �����
    [SerializeField] private float spawnRadius = 5f;    // ������ ���������� ��������
    [SerializeField] private float spawnInterval = 10f; // �������� ������ (� ��������)

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("�� ������� ����� ������!");
            return;
        }

        InvokeRepeating(nameof(SpawnDiamond), spawnInterval, spawnInterval);
    }

    private void SpawnDiamond()
    {
        if (!diamondPrefab) return;

        // �������� ��������� ����� ������
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // �������� ��������� �������� � �������
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0; // ����� ����� �� ������ �����/����

        // ������� �����
        Instantiate(diamondPrefab, randomPoint.position + randomOffset, Quaternion.identity);
    }
}
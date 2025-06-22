using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("Prefab Spawn Settings")]
    [SerializeField] private GameObject[] prefabsToSpawn; // ������ ��������, ������� ����� ��������
    [SerializeField] private int minPrefabsToSpawn = 3; // ����������� ���������� ��������
    [SerializeField] private int maxPrefabsToSpawn = 7; // ������������ ���������� ��������
    [SerializeField] private float spawnForce = 5f; // ����, � ������� �������� �������

    private int currentHealth;

    public event System.Action OnDeath;
    public event System.Action<int> OnDamageTaken;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnDamageTaken?.Invoke(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        SpawnPrefabs();
        Destroy(gameObject);
    }

    private void SpawnPrefabs()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("No prefabs to spawn assigned!");
            return;
        }

        // ��������� ���������� �������� ��� ������
        int numToSpawn = Random.Range(minPrefabsToSpawn, maxPrefabsToSpawn + 1);
        CameraFollow.ShakeCamera(0.4f, 0.3f);

        for (int i = 0; i < numToSpawn; i++)
        {
            // �������� ��������� ������ �� �������
            GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

            // ������� ��������� �������
            GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);

            // ��������� Rigidbody, ���� ��� ���
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = instance.AddComponent<Rigidbody>();
            }

            // ���������� ��������� �����������
            Vector3 randomDirection = Random.insideUnitSphere.normalized;

            // ��������� ���� � ���� �����������
            rb.AddForce(randomDirection * spawnForce, ForceMode.Impulse);

            // ��������� ��������� ������������ ������
            rb.AddTorque(Random.insideUnitSphere * spawnForce, ForceMode.Impulse);
        }
    }
}
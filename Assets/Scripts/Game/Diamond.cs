using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private float detectRadius = 2f; // ���������� ����������� ������
    [SerializeField] private LayerMask playerLayer;    // ���� ������ ��� ��������

    private bool collected = false;

    private void Update()
    {
        if (collected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            // ����� ������ � �������� �����
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;
        // ����� �������� ������ ����� (����, �������� � �.�.)
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
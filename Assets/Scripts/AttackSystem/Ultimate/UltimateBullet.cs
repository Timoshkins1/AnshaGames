using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UltimateBullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f;
    public float lifetime = 2f;
    public int damage = 30; // ������� ���� (������� ���� = 10)
    public float knockbackForce = 15f;
    public LayerMask destroyableLayers; // Bush, Obstacle, Destroyable

    private Rigidbody rb;
    private GameObject owner;

    public void Initialize(Vector3 direction, GameObject owner)
    {
        rb = GetComponent<Rigidbody>();
        this.owner = owner;

        rb.velocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        // ���������� ������� �� ��������� ����
        if (((1 << other.gameObject.layer) & destroyableLayers) != 0)
        {
            Destroy(other.gameObject); // ������� �����������
            CameraFollow.ShakeCamera(0.9f, 0.3f);
        }

        // ������� ���� ������/��������
        if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
            ApplyKnockback(other);
        }
        else if (other.TryGetComponent<ObjectHealth>(out var objectHealth))
        {
            objectHealth.TakeDamage(damage);
            ApplyKnockback(other);
        }

        // �� ���������� ���� ���� (��������� ������)
    }

    private void ApplyKnockback(Collider target)
    {
        if (target.TryGetComponent<Rigidbody>(out var rb))
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }
    }
}
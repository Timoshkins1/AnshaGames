using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Ignore Settings")]
    public LayerMask ignoreLayers; // ���� ��� �������������
    public LayerMask destroyLayers; // ���� �����������
    public string[] ignoreTags = { "ObstacleNonCollision", "Water" }; // ���� ��� �������������

    private Vector3 direction;
    private float speed;
    private float lifetime;
    private int damage;
    private float knockbackForce;
    private GameObject owner;
    private Rigidbody rb;


    public void Initialize(Vector3 dir, float spd, float life, int dmg, float knockback, GameObject ownerObj)
    {
        direction = dir;
        speed = spd;
        lifetime = life;
        damage = dmg;
        knockbackForce = knockback;
        owner = ownerObj;

        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        Destroy(gameObject, lifetime);

        if (rb.velocity != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���������� ������������ � ����������
        if (other.gameObject == owner) return;

        // ���������, ����� �� ������������ ���� ������
        if (ShouldIgnoreCollision(other))
        {
            return;
        }
        if (other.gameObject == (destroyLayers == (destroyLayers | (1 << other.gameObject.layer)))) Destroy(gameObject);

        // ��������� ���������
        if (other.TryGetComponent<EnemyHealth>(out var health))
        {
            health.TakeDamage(damage);

            if (other.TryGetComponent<Rigidbody>(out var targetRb))
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
        // ��������� ��������� �� �������
        if (other.TryGetComponent< ObjectHealth>(out var healthObj))
        {
            health.TakeDamage(damage);

            if (other.TryGetComponent<Rigidbody>(out var targetRb))
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    private bool ShouldIgnoreCollision(Collider other)
    {
        // �������� �� �����
        if (ignoreLayers == (ignoreLayers | (1 << other.gameObject.layer)))
        {
            return true;
        }

        // �������� �� �����
        foreach (string tag in ignoreTags)
        {
            if (other.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }
}
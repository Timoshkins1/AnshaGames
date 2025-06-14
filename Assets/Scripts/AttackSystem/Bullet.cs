using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Ignore Settings")]
    public LayerMask ignoreLayers;
    public LayerMask destroyLayers;
    public string[] ignoreTags = { "ObstacleNonCollision", "Water" };

    private Vector3 direction;
    private float speed;
    private int damage;
    private float knockbackForce;
    private GameObject owner;
    private Rigidbody rb;

    public void Initialize(Vector3 dir, float spd, float life, int dmg, float knockback, GameObject ownerObj)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
        knockbackForce = knockback;
        owner = ownerObj;
        rb = GetComponent<Rigidbody>();

        rb.velocity = direction * speed;
        Destroy(gameObject, life);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;
        if (ShouldIgnoreCollision(other)) return;

        bool shouldDestroy = (destroyLayers.value & (1 << other.gameObject.layer)) != 0;

        if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
            ApplyKnockback(other);
            shouldDestroy = true;
        }
        else if (other.TryGetComponent<ObjectHealth>(out var objectHealth))
        {
            objectHealth.TakeDamage(damage);
            ApplyKnockback(other);
            shouldDestroy = true;
        }

        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
    }

    private void ApplyKnockback(Collider target)
    {
        if (target.TryGetComponent<Rigidbody>(out var targetRb))
        {
            Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;
            targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    private bool ShouldIgnoreCollision(Collider other)
    {
        // Проверка по слоям
        if ((ignoreLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            return true;
        }

        // Проверка по тегам
        var otherTag = other.tag;
        foreach (string tag in ignoreTags)
        {
            if (otherTag == tag)
            {
                return true;
            }
        }

        return false;
    }
}
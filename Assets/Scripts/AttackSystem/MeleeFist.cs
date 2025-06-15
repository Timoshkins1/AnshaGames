using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MeleeFist : MonoBehaviour
{
    [Header("Ignore Settings")]
    public LayerMask ignoreLayers;
    public LayerMask destroyLayers;
    public string[] ignoreTags = { "ObstacleNonCollision", "Water" };

    private Vector3 direction;
    private float range;
    private float damage;
    private float knockbackForce;
    private float duration;
    private float timer;
    private Vector3 startPosition;
    private GameObject owner;
    private Rigidbody rb;
    private bool hasHit = false;
    private bool isReturning = false;

    public void Initialize(Vector3 attackDirection, float attackRange, float attackDamage, float attackKnockback, float attackDuration, GameObject ownerObj)
    {
        direction = attackDirection;
        range = attackRange;
        damage = attackDamage;
        knockbackForce = attackKnockback;
        duration = attackDuration;
        owner = ownerObj;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (hasHit && !isReturning)
        {
            isReturning = true;
            timer = duration * 0.5f;
        }

        if (!hasHit)
        {
            float progress = Mathf.Clamp01(timer / (duration * 0.5f));
            transform.position = startPosition + direction * range * progress;
        }

        if (timer > duration * 0.5f)
        {
            float returnProgress = Mathf.Clamp01((timer - duration * 0.5f) / (duration * 0.5f));
            transform.position = Vector3.Lerp(
                startPosition + direction * range,
                startPosition,
                returnProgress
            );
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;
        if (ShouldIgnoreCollision(other)) return;
        if (hasHit) return;

        bool shouldDestroy = (destroyLayers.value & (1 << other.gameObject.layer)) != 0;

        if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
            ApplyKnockback(other);
            hasHit = true;
        }
        else if (other.TryGetComponent<ObjectHealth>(out var objectHealth))
        {
            objectHealth.TakeDamage((int)damage);
            ApplyKnockback(other);
            hasHit = true;
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
        if ((ignoreLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            return true;
        }

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
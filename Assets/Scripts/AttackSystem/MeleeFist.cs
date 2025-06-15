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

    // Поля для работы с системой патронов
    private int currentAmmo;
    private int maxAmmo;
    private float reloadTime;
    private bool isReloading = false;

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

        // Поворачиваем модель кулака в направлении удара
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void InitializeWithAmmo(Vector3 attackDirection, float attackRange, float attackDamage, float attackKnockback, float attackDuration, GameObject ownerObj, PlayerAttack attackConfig)
    {
        Initialize(attackDirection, attackRange, attackDamage, attackKnockback, attackDuration, ownerObj);

        // Инициализация параметров патронов из конфига атаки
        maxAmmo = attackConfig.maxAmmo;
        reloadTime = attackConfig.reloadTime;
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Если кулак уже ударил и начал возвращаться, не двигаем его дальше
        if (hasHit && !isReturning)
        {
            isReturning = true;
            timer = duration * 0.5f; // Переходим сразу к фазе возвращения
        }

        // Движение кулака вперед (только если еще не ударил)
        if (!hasHit)
        {
            float progress = Mathf.Clamp01(timer / (duration * 0.5f));
            transform.position = startPosition + direction * range * progress;
        }

        // Возвращение кулака (вторая половина длительности)
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

        // Логика перезарядки
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public bool CanAttack()
    {
        return currentAmmo > 0 && !isReloading;
    }

    public void ConsumeAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
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
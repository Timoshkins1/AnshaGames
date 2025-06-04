using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int _damage;

    public void SetDamage(int damage) => _damage = damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(_damage);
        }
        Destroy(gameObject);
    }
}
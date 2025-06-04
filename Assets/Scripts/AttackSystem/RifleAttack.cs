using UnityEngine;

public class RangedAttack : PlayerAttack
{
    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public int damage = 10;

    protected override void ExecuteAttack()
    {
        Vector3 direction = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical).normalized;
        GameObject projectile = Instantiate(projectilePrefab, _attackPoint.position, Quaternion.LookRotation(direction));

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = direction * projectileSpeed;
        }

        if (projectile.TryGetComponent<Projectile>(out var proj))
        {
            proj.SetDamage(damage);
        }
    }
}
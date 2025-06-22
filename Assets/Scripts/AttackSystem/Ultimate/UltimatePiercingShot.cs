using UnityEngine;

public class UltimatePiercingShot : PlayerUltimate
{
    [Header("Ultimate Bullet Settings")]
    [SerializeField] private GameObject ultimateBulletPrefab;
    [SerializeField] private float bulletSpeed = 25f;
    [SerializeField] private float bulletLifetime = 1.5f;
    [SerializeField] private LayerMask destroyableLayers;

    public override void ActivateUltimate()
    {
        if (ultimateBulletPrefab == null) return;

        var shooting = GetComponent<PlayerShooting>();
        if (shooting == null) return;

        Vector3 direction = shooting.GetLastAttackDirection();

        
        var bullet = Instantiate(
            ultimateBulletPrefab,
            shooting.firePoint.position,
            Quaternion.LookRotation(direction)
        );

        var ultimateBullet = bullet.GetComponent<UltimateBullet>();
        if (ultimateBullet != null)
        {
            ultimateBullet.speed = bulletSpeed;
            ultimateBullet.lifetime = bulletLifetime;
            ultimateBullet.destroyableLayers = destroyableLayers;
            ultimateBullet.Initialize(direction, gameObject);
        }
    }
}
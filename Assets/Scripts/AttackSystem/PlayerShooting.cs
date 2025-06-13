using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public PlayerAttack attackConfig;
    public Transform firePoint;
    public Joystick joystick;
    public float joystickDeadZone = 0.1f;

    private bool isAiming = false;
    private Vector3 shootDirection;

    private void Update()
    {
        HandleJoystickInput();
    }

    private void HandleJoystickInput()
    {
        Vector3 joystickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (joystickDirection.magnitude > joystickDeadZone)
        {
            isAiming = true;
            shootDirection = joystickDirection.normalized;
        }
        else if (isAiming)
        {
            Shoot();
            isAiming = false;
        }
    }

    private void Shoot()
    {
        if (attackConfig.bulletsCount <= 0) return;

        float angleStep = attackConfig.spreadAngle / (attackConfig.bulletsCount - 1);
        float startAngle = -attackConfig.spreadAngle / 2f;

        for (int i = 0; i < attackConfig.bulletsCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector3 bulletDirection = RotateVectorXZ(shootDirection, currentAngle);
            CreateBullet(bulletDirection);
        }
    }

    private void CreateBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(attackConfig.bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        bulletScript.Initialize(
            direction,
            attackConfig.bulletSpeed,
            attackConfig.bulletLifetime,
            attackConfig.damagePerBullet,
            attackConfig.knockbackForce,
            gameObject
        );
    }

    private Vector3 RotateVectorXZ(Vector3 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(
            vector.x * Mathf.Cos(rad) - vector.z * Mathf.Sin(rad),
            0,
            vector.x * Mathf.Sin(rad) + vector.z * Mathf.Cos(rad)
        ).normalized;
    }
}
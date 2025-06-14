using UnityEngine;
using System.Collections.Generic;

using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public PlayerAttack attackConfig;
    public Transform firePoint;
    public Joystick joystick;
    public float joystickDeadZone = 0.1f;

    [Header("Targeting")]
    public LayerMask targetLayer;
    public float targetSearchRadius = 10f;

    [Header("UI References")]
    public AmmoDisplay ammoDisplay;

    [Header("Shooting Settings")]
    public float shotDelay = 0.2f; // Добавляем настраиваемую задержку между выстрелами

    private bool isAiming = false;
    private Vector3 shootDirection;
    private Collider[] targetCollidersCache = new Collider[20];
    private Transform nearestTarget;

    // Система патронов и таймеры
    private int currentAmmo;
    private float reloadTimer;
    private float shotCooldownTimer;
    private bool isReloading = false;
    private bool canShoot = true;

    private void Start()
    {
        currentAmmo = attackConfig.maxAmmo;
        if (ammoDisplay != null)
        {
            ammoDisplay.Initialize(attackConfig.maxAmmo); // Важно: сначала инициализация!
        }
        UpdateAmmoDisplay();
    }
    private void Update()
    {
        HandleTimers();
        HandleJoystickInput();
    }

    private void HandleJoystickInput()
    {
        Vector3 joystickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (joystickDirection.sqrMagnitude > joystickDeadZone * joystickDeadZone)
        {
            isAiming = true;
            shootDirection = joystickDirection.normalized;
        }
        else if (isAiming && canShoot) // Добавляем проверку canShoot
        {
            if (currentAmmo > 0)
            {
                FindNearestTarget();
                Shoot();

                // Обновляем состояние стрельбы
                canShoot = false;
                currentAmmo--;
                UpdateAmmoDisplay();

                if (!isReloading)
                {
                    isReloading = true;
                    reloadTimer = 0f;
                }
            }
            isAiming = false;
        }
    }

    private void HandleTimers()
    {
        // Обработка перезарядки
        if (currentAmmo < attackConfig.maxAmmo)
        {
            reloadTimer += Time.deltaTime;

            // Сообщаем AmmoDisplay о перезарядке
            if (ammoDisplay != null && !isReloading)
            {
                ammoDisplay.SetReloadingState(true);
            }

            if (reloadTimer >= attackConfig.reloadTime)
            {
                currentAmmo++;
                reloadTimer = 0f;
                UpdateAmmoDisplay();

                if (currentAmmo >= attackConfig.maxAmmo)
                {
                    isReloading = false;
                    if (ammoDisplay != null)
                    {
                        ammoDisplay.SetReloadingState(false);
                    }
                }
            }
        }

        // Обработка задержки между выстрелами остается без изменений
        if (!canShoot)
        {
            shotCooldownTimer += Time.deltaTime;
            if (shotCooldownTimer >= shotDelay)
            {
                canShoot = true;
                shotCooldownTimer = 0f;
            }
        }
    }
    private void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.UpdateAmmo(currentAmmo);
        }
    }
    private void FindNearestTarget()
    {
        nearestTarget = null;
        int targetsCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            targetSearchRadius,
            targetCollidersCache,
            targetLayer
        );

        if (targetsCount == 0) return;

        float minDistance = float.MaxValue;
        for (int i = 0; i < targetsCount; i++)
        {
            float distance = (transform.position - targetCollidersCache[i].transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = targetCollidersCache[i].transform;
            }
        }

        if (nearestTarget != null)
        {
            shootDirection = (nearestTarget.position - firePoint.position).normalized;
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
        var bullet = Instantiate(
            attackConfig.bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        bullet.GetComponent<Bullet>().Initialize(
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
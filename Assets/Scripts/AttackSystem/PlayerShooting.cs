using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public enum AttackType { Ranged, Melee }

    [Header("Attack Configuration")]
    public AttackType attackType = AttackType.Ranged;
    public PlayerAttack attackConfig;

    [Header("Animation References")]
    [SerializeField] private PlayerAnimation _playerAnimation;

    [Header("Ranged Settings")]
    public Transform firePoint;
    public float joystickDeadZone = 0.1f;
    public LayerMask targetLayer;
    public float targetSearchRadius = 10f;

    [Header("Melee Settings")]
    public GameObject fistPrefab;
    public float meleeRange = 1.5f;
    public float meleeDuration = 0.3f;
    public Vector3 fistSpawnOffset = new Vector3(0, 0, 0.5f);

    [Header("UI References")]
    public Joystick joystick;
    public AmmoDisplay ammoDisplay;

    [Header("Animation Settings")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private Transform model; // ������ �� ������ ��� ��������

    [Header("Timing Settings")]
    public float shotDelay = 0.2f;

    private bool isAiming = false;
    private Vector3 attackDirection;
    private int currentAmmo;
    private float shotCooldownTimer;
    private bool canShoot = true;
    private Collider[] targetCollidersCache = new Collider[20];
    private bool isReloading = false;
    private float reloadTimer;

    private void Start()
    {
        currentAmmo = attackConfig.maxAmmo;
        InitializeAmmoDisplay();
    }

    public void Initialize(Joystick joystick, AmmoDisplay ammoDisplay)
    {
        this.joystick = joystick;
        this.ammoDisplay = ammoDisplay;
        InitializeAmmoDisplay();
    }

    private void InitializeAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.Initialize(attackConfig.maxAmmo);
            ammoDisplay.UpdateAmmo(currentAmmo);
            ammoDisplay.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        HandleCooldown();
        HandleAttackInput();
        HandleReload();
    }

    private void HandleReload()
    {
        if (currentAmmo < attackConfig.maxAmmo && !isReloading)
        {
            isReloading = true;
            reloadTimer = attackConfig.reloadTime;
            if (ammoDisplay != null)
            {
                ammoDisplay.SetReloadingState(true);
            }
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                currentAmmo++;
                UpdateAmmoDisplay();
                isReloading = false;

                if (ammoDisplay != null)
                {
                    ammoDisplay.SetReloadingState(false);
                }

                // ���� ��� �� ������ ��������, �������� ����������� ���������� ������
                if (currentAmmo < attackConfig.maxAmmo)
                {
                    isReloading = true;
                    reloadTimer = attackConfig.reloadTime;
                    if (ammoDisplay != null)
                    {
                        ammoDisplay.SetReloadingState(true);
                    }
                }
            }
        }
    }

    private void HandleCooldown()
    {
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

    private void HandleAttackInput()
    {
        Vector3 joystickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (joystickDirection.sqrMagnitude > joystickDeadZone * joystickDeadZone)
        {
            isAiming = true;
            attackDirection = joystickDirection.normalized;
           
        }
        else if (isAiming && canShoot && currentAmmo > 0)
        {
            if (attackType == AttackType.Ranged)
            {
                PerformRangedAttack();
            }
            else if (attackType == AttackType.Melee)
            {
                PerformMeleeAttack();
            }

            currentAmmo--;
            UpdateAmmoDisplay();
            canShoot = false;
            isAiming = false;
        }
        else if (isAiming && currentAmmo <= 0)
        {
            isAiming = false;
        }
    }

    private void PerformRangedAttack()
    {
        FindNearestTarget();

        if (attackConfig.bulletsCount > 0)
        {
            float angleStep = attackConfig.spreadAngle / (attackConfig.bulletsCount - 1);
            float startAngle = -attackConfig.spreadAngle / 2f;

            for (int i = 0; i < attackConfig.bulletsCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Vector3 bulletDirection = RotateVectorXZ(attackDirection, currentAngle);
                CreateBullet(bulletDirection);
            }
        }

        // �������� ��������� �������� ������������ �����
        if (_playerAnimation != null)
        {
            _playerAnimation.TriggerRangedAttack();
        }

        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.HandleAttackRotation(attackDirection);
        }
    }

    private void PerformMeleeAttack()
    {
        FindNearestTarget();
        Vector3 spawnPosition = transform.position + transform.TransformDirection(fistSpawnOffset);
        GameObject fist = Instantiate(fistPrefab, spawnPosition, Quaternion.identity);

        if (fist.TryGetComponent<MeleeFist>(out var meleeFist))
        {
            meleeFist.Initialize(
                attackDirection.normalized,
                meleeRange,
                attackConfig.damagePerBullet,
                attackConfig.knockbackForce,
                meleeDuration,
                gameObject
            );
        }
        else
        {
            Destroy(fist, meleeDuration);
        }

        // �������� ��������� �������� ������� �����
        if (_playerAnimation != null)
        {
            _playerAnimation.TriggerMeleeAttack();
        }

        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.HandleAttackRotation(attackDirection);
        }
    }
    public void UpdateAmmoCapacity()
    {
        currentAmmo = attackConfig.maxAmmo;
        UpdateAmmoDisplay();
    }
    private void FindNearestTarget()
    {
        Transform nearestTarget = null;
        int targetsCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            targetSearchRadius,
            targetCollidersCache,
            targetLayer
        );

        if (targetsCount > 0)
        {
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
                attackDirection = (nearestTarget.position - firePoint.position).normalized;
            }
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

    private void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.UpdateAmmo(currentAmmo);
        }
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

    public void SetAttackType(AttackType type)
    {
        attackType = type;
    }
}
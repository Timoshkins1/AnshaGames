using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public enum AttackType { Ranged, Melee }

    [Header("Attack Configuration")]
    public AttackType attackType = AttackType.Ranged;
    public PlayerAttack attackConfig;

    [Header("Input Settings")]
    public float tapThreshold = 0.2f; // Максимальное время для распознавания тапа
    private float joystickActiveTime = 0f;
    private bool isJoystickHeld = false;

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
    [SerializeField] private Transform model; // Ссылка на модель для поворота

    [Header("Timing Settings")]
    public float shotDelay = 0.2f;

    public Vector3 GetLastAttackDirection() => attackDirection;
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

                // Если еще не полный боезапас, начинаем перезарядку следующего заряда
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
        bool joystickActive = joystickDirection.sqrMagnitude > joystickDeadZone * joystickDeadZone;

        // Обновляем таймер активности джойстика
        if (joystickActive)
        {
            if (!isJoystickHeld)
            {
                // Джойстик только что начал двигаться
                isJoystickHeld = true;
                joystickActiveTime = 0f;
            }
            else
            {
                // Джойстик продолжает двигаться
                joystickActiveTime += Time.deltaTime;
            }

            // Всегда обновляем направление при активном джойстике
            attackDirection = joystickDirection.normalized;
        }
        else if (isJoystickHeld)
        {
            // Джойстик отпустили - определяем тип ввода
            if (joystickActiveTime <= tapThreshold)
            {
                // Быстрое нажатие - атака по ближайшему врагу
                FindNearestTarget();
                TryAttack();
            }
            else
            {
                // Долгое зажатие - атака в последнем направлении
                TryAttack();
            }

            isJoystickHeld = false;
            joystickActiveTime = 0f;
        }
    }
    private void TryAttack()
    {
        if (!canShoot) return;

        // Проверяем, заряжена ли ульта
        var ultimate = GetComponent<PlayerUltimate>();
        if (ultimate != null && ultimate.IsUltimateReady())
        {
            ultimate.ActivateUltimate();
            CameraFollow.ShakeCamera(0.6f, 0.2f);
            ultimate.ResetCharge();
            canShoot = false;
            return;
        }

        if (currentAmmo <= 0) return;

        // Выполняем обычную атаку
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
    }
    private void PerformRangedAttack()
    {
    

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

        // Вызываем случайную анимацию дальнобойной атаки
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
                gameObject,
                GetComponent<PlayerUltimate>()
            );
        }
        else
        {
            Destroy(fist, meleeDuration);
        }

        // Вызываем случайную анимацию ближней атаки
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
            gameObject,
            GetComponent<PlayerUltimate>()
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
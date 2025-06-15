using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    private FixedJoystick moveJoystick;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform model;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Vector3 modelRotationOffset = new Vector3(0, -90, 0);
    [SerializeField] private bool useKeyboardInput = true; // Добавлено: переключатель управления

    [Header("Bush Boost Settings")]
    [SerializeField] private float bushSpeedMultiplier = 1.2f;
    [SerializeField] private float boostFadeDuration = 0.5f;
    [SerializeField] private float bushCheckInterval = 0.2f;
    [SerializeField] private LayerMask bushLayer;

    [Header("Attack Rotation Settings")]
    [SerializeField] private float attackRotationSpeed = 20f;
    [SerializeField] private float attackLookInfluence = 0.7f; // Влияние направления атаки на поворот (0-1)
    [SerializeField] private float attackLookDuration = 0.5f;

    private Vector3 lastAttackDirection;
    private float attackLookTimer;
    private bool hasRecentAttack;

    private Rigidbody rb;
    private float currentSpeed;
    private float baseSpeed;
    private bool isInBush;
    private float boostTimer;
    private float lastBushCheckTime;
    private Collider[] bushCheckCache = new Collider[1];

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        baseSpeed = moveSpeed;
        currentSpeed = baseSpeed;

        if (moveJoystick == null && PlayerManager.Instance != null)
        {
            moveJoystick = PlayerManager.Instance.MovementJoystick;
        }
    }

    public void HandleAttackRotation(Vector3 attackDirection)
    {
        lastAttackDirection = attackDirection;
        attackLookTimer = attackLookDuration;
        hasRecentAttack = true;
    }

    public void Initialize(FixedJoystick joystick)
    {
        moveJoystick = joystick;
    }

    private void FixedUpdate()
    {
        HandleBushCheck();
        UpdateSpeed();
        HandleMovement();
    }

    private void HandleBushCheck()
    {
        if (Time.time - lastBushCheckTime >= bushCheckInterval)
        {
            lastBushCheckTime = Time.time;
            isInBush = Physics.OverlapSphereNonAlloc(
                transform.position,
                0.5f,
                bushCheckCache,
                bushLayer
            ) > 0;
        }
    }

    private void UpdateSpeed()
    {
        if (isInBush)
        {
            currentSpeed = baseSpeed * bushSpeedMultiplier;
            boostTimer = boostFadeDuration;
        }
        else if (boostTimer > 0)
        {
            boostTimer -= Time.fixedDeltaTime;
            currentSpeed = Mathf.Lerp(baseSpeed, baseSpeed * bushSpeedMultiplier, boostTimer / boostFadeDuration);
        }
        else
        {
            currentSpeed = baseSpeed;
        }
    }
    public void IncreaseMoveSpeed(float amount)
    {
        baseSpeed += amount;
        moveSpeed = baseSpeed;
    }

    public void IncreaseBushMultiplier(float amount)
    {
        bushSpeedMultiplier += amount;
    }

    public void IncreaseBoostFadeDuration(float amount)
    {
        boostFadeDuration += amount;
    }
    private void HandleMovement()
    {
        Vector2 joystickInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        // Обновляем таймер атаки
        if (hasRecentAttack)
        {
            attackLookTimer -= Time.fixedDeltaTime;
            if (attackLookTimer <= 0)
            {
                hasRecentAttack = false;
            }
        }

        // Обработка движения
        if (joystickInput.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
            rb.velocity = new Vector3(
                moveDirection.x * currentSpeed,
                rb.velocity.y,
                moveDirection.z * currentSpeed
            );

            // Определяем целевое направление поворота
            Vector3 targetLookDirection = moveDirection;

            // Если была недавняя атака, смешиваем направления
            if (hasRecentAttack)
            {
                float influence = Mathf.Clamp01(attackLookInfluence * (attackLookTimer / attackLookDuration));
                targetLookDirection = Vector3.Lerp(moveDirection, lastAttackDirection, influence).normalized;
            }

            // Поворот
            if (targetLookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetLookDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    (hasRecentAttack ? attackRotationSpeed : rotationSpeed) * Time.fixedDeltaTime
                );
            }
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            // Если стоит на месте, поворачиваемся полностью в сторону атаки
            if (hasRecentAttack && lastAttackDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lastAttackDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    attackRotationSpeed * Time.fixedDeltaTime
                );
            }
        }

        // Поворот модели
        if (model != null)
        {
            model.rotation = transform.rotation * Quaternion.Euler(modelRotationOffset);
        }
    }
}
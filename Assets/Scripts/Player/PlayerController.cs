using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    private FixedJoystick moveJoystick;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform model;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Vector3 modelRotationOffset = new Vector3(0, -90, 0); // ���������: ������������� ������� ������

    [Header("Bush Boost Settings")]
    [SerializeField] private float bushSpeedMultiplier = 1.2f;
    [SerializeField] private float boostFadeDuration = 0.5f;
    [SerializeField] private float bushCheckInterval = 0.2f; // �����������: ��������� ����
    [SerializeField] private LayerMask bushLayer;

    private Rigidbody rb;
    private float currentSpeed;
    private float baseSpeed;
    private bool isInBush;
    private float boostTimer;
    private float lastBushCheckTime;
    private Collider[] bushCheckCache = new Collider[1]; // ��� ��� �������� ������


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
        // �����������: ��������� ������� ������ �� ������ ����
        if (Time.time - lastBushCheckTime >= bushCheckInterval)
        {
            lastBushCheckTime = Time.time;
            isInBush = Physics.OverlapSphereNonAlloc(
                transform.position,
                0.5f, // ������ ��������
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

    private void HandleMovement()
    {
        Vector2 joystickInput = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical);

        // �����������: ���������� sqrMagnitude ��� �������� �����
        if (joystickInput.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
            rb.velocity = new Vector3(
                moveDirection.x * currentSpeed,
                rb.velocity.y,
                moveDirection.z * currentSpeed
            );

            // �����������: ������� ������ ���� ����������� ����������
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        if (model != null)
        {
            model.rotation = transform.rotation * Quaternion.Euler(modelRotationOffset);
        }
    }
}
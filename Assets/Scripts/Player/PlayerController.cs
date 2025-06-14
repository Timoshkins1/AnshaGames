using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private FixedJoystick _moveJoystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model; // Модель (если нужен отдельный поворот)
    [SerializeField] private float _rotationSpeed = 10f;
    [Header("Bush Speed Boost")]
    [SerializeField] private float _bushSpeedMultiplier = 1.2f; // 20% увеличение скорости
    [SerializeField] private float _boostFadeDuration = 0.5f; // Плавное исчезновение эффекта

    private Rigidbody _rb;
    private float _currentSpeed;
    private float _baseSpeed;
    private bool _isInBush = false;
    private float _boostTimer = 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; // Запрещаем физическому движку вращать объект
        _baseSpeed = _moveSpeed;
        _currentSpeed = _baseSpeed;
    }

    private void Start()
    {
        int characterID = PlayerPrefs.GetInt("SelectedCharacterID", 1);
    }

    private void FixedUpdate()
    {
        UpdateSpeed();
        HandleMovement();
    }

    private void UpdateSpeed()
    {
        if (_isInBush)
        {
            _currentSpeed = _baseSpeed * _bushSpeedMultiplier;
            _boostTimer = _boostFadeDuration; // Сбрасываем таймер, пока в кустах
        }
        else if (_boostTimer > 0)
        {
            _boostTimer -= Time.fixedDeltaTime;
            // Плавное уменьшение скорости после выхода из кустов
            _currentSpeed = Mathf.Lerp(_baseSpeed, _baseSpeed * _bushSpeedMultiplier, _boostTimer / _boostFadeDuration);
        }
        else
        {
            _currentSpeed = _baseSpeed;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveInput = new Vector3(_moveJoystick.Horizontal, 0, _moveJoystick.Vertical);

        if (moveInput.magnitude > 0.1f)
        {
            // Нормализуем и применяем движение
            moveInput = moveInput.normalized;
            _rb.velocity = new Vector3(moveInput.x * _currentSpeed, _rb.velocity.y, moveInput.z * _currentSpeed);

            // Поворот игрока в сторону движения (только по оси Y)
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Останавливаем движение, но сохраняем вертикальную скорость (например, для гравитации)
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }

        // Модель всегда повторяет поворот игрока с offset -90 по Y
        if (_model != null)
        {
            _model.rotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            _isInBush = true;
            Debug.Log("Entered bush - speed increased!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            _isInBush = false;
            Debug.Log("Exited bush - speed will return to normal");
        }
    }
}
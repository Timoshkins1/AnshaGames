using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private FixedJoystick _moveJoystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model; // Модель (если нужен отдельный поворот)
    [SerializeField] private float _rotationSpeed = 10f;

    private Rigidbody _rb;
 

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        int characterID = PlayerPrefs.GetInt("SelectedCharacterID", 1);
    }

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(_moveJoystick.Horizontal, 0, _moveJoystick.Vertical);
        if (moveInput.magnitude > 0.1f)
        {
            // Нормализуем и применяем движение
            moveInput = moveInput.normalized;
            _rb.velocity = moveInput * _moveSpeed;

            // Поворот игрока в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.deltaTime));
        }
        else
        {
            // Останавливаем инерцию
            _rb.angularVelocity = Vector3.zero;
            _rb.velocity = Vector3.zero;
        }

        // Модель всегда повторяет поворот игрока с offset -90 по Y
        if (_model != null)
        {
            _model.rotation = _rb.rotation * Quaternion.Euler(0, -90, 0);
        }
    }
}
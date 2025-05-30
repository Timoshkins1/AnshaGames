using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model;
    [SerializeField] private float _rotationCorrection = -90f;


    [Header("References")]
    [SerializeField] private SpawnSystem _spawnSystem;

    private Health _health;
    private bool _isActive = true;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDeath += HandleDeath;

        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;

        Vector3 moveInput = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
        _rb.velocity = moveInput.normalized * _moveSpeed;

        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput) * Quaternion.Euler(0, _rotationCorrection, 0);
            _model.rotation = targetRotation;
        }
        else
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void HandleDeath()
    {
        _isActive = false;
        _rb.velocity = Vector3.zero;
        _spawnSystem.RespawnPlayer(gameObject);
    }

    public void OnEnable()
    {
        _isActive = true;
    }
}
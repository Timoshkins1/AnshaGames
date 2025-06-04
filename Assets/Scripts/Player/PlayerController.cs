using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private FixedJoystick _moveJoystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private FixedJoystick _attackJoystick;
    [SerializeField] private Transform _attackOrigin;

    private Rigidbody _rb;
    private PlayerAttack _attackSystem;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _attackSystem = GetComponent<PlayerAttack>();

        // Инициализация системы атаки
        if (_attackSystem != null)
        {
            _attackSystem.Initialize(_attackJoystick, _attackOrigin);
        }
    }
    private void Start()
    {
        int characterID = PlayerPrefs.GetInt("SelectedCharacterID", 1);

        // Удаляем старые атаки
        var oldAttack = GetComponent<PlayerAttack>();
        if (oldAttack != null) Destroy(oldAttack);

        // Добавляем нужный тип
        switch (characterID)
        {
            case 1: gameObject.AddComponent<MeleeAttack>(); break;
            case 2: gameObject.AddComponent<ShotgunAttack>(); break;
            case 3: gameObject.AddComponent<RangedAttack>(); break;
        }
    }
    private void FixedUpdate()
    {
        // Движение
        Vector3 moveInput = new Vector3(_moveJoystick.Horizontal, 0, _moveJoystick.Vertical);
        _rb.velocity = moveInput.normalized * _moveSpeed;

        // Поворот модели
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            _model.rotation = Quaternion.Slerp(_model.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}
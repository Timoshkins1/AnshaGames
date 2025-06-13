using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private FixedJoystick _moveJoystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model; // ������ (���� ����� ��������� �������)
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
            // ����������� � ��������� ��������
            moveInput = moveInput.normalized;
            _rb.velocity = moveInput * _moveSpeed;

            // ������� ������ � ������� ��������
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.deltaTime));
        }
        else
        {
            // ������������� �������
            _rb.angularVelocity = Vector3.zero;
            _rb.velocity = Vector3.zero;
        }

        // ������ ������ ��������� ������� ������ � offset -90 �� Y
        if (_model != null)
        {
            _model.rotation = _rb.rotation * Quaternion.Euler(0, -90, 0);
        }
    }
}
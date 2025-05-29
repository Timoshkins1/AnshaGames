using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model; // ������ �� �������� ������
    [SerializeField] private float _rotationCorrection = -90f; // ��������� ���� (����� ������ � ����������)

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
        _rb.velocity = moveInput.normalized * _moveSpeed;

        if (moveInput != Vector3.zero)
        {
            // ������������ ������ � ���������� ����
            Quaternion targetRotation = Quaternion.LookRotation(moveInput) * Quaternion.Euler(0, _rotationCorrection, 0);
            _model.rotation = targetRotation;
        }
        else
        {
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}
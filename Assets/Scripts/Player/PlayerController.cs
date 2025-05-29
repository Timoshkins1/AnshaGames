using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Transform _model; // Ссылка на дочернюю модель
    [SerializeField] private float _rotationCorrection = -90f; // Коррекция угла (можно менять в инспекторе)

    private void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
        _rb.velocity = moveInput.normalized * _moveSpeed;

        if (moveInput != Vector3.zero)
        {
            // Поворачиваем модель с коррекцией угла
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
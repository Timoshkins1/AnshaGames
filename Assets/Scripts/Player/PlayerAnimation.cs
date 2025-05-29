using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private float _minInputThreshold = 0.1f; // ����� ���������������� ���������

    private void Update()
    {
        // ���������, ������� �� �������� (���� ����)
        bool isRunning = _joystick.Horizontal != 0 || _joystick.Vertical != 0;

        // �������� �������� � Animator
        _animator.SetBool("IsRunning", isRunning);
    }
}
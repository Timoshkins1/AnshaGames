using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private FixedJoystick _joystick;

    private void Update()
    {
        // ���������, ������� �� �������� (���� ����)
        bool isRunning = _joystick.Horizontal != 0 || _joystick.Vertical != 0;

        // �������� �������� � Animator
        _animator.SetBool("IsRunning", isRunning);
    }
}
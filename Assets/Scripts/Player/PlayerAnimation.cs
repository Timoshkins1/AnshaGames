using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    public FixedJoystick _joystick;
    private bool _initialized = false;
    private PlayerShooting _playerShooting;

    public void Initialize(FixedJoystick joystick)
    {
        _joystick = joystick;
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator not found in children!", this);
            return;
        }

        if (_joystick == null)
        {
            Debug.LogError("Joystick reference is null!", this);
            return;
        }

        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized) return;

        // Безопасная проверка ввода
        bool isRunning = (_joystick != null) &&
                        (_joystick.Horizontal != 0 || _joystick.Vertical != 0);

        // Безопасное обновление аниматора
        if (_animator != null && _animator.isActiveAndEnabled)
        {
            _animator.SetBool("IsRunning", isRunning);
        }
    }
}
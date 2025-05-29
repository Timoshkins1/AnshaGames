using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private float _minInputThreshold = 0.1f; // Порог чувствительности джойстика

    private void Update()
    {
        // Проверяем, активен ли джойстик (есть ввод)
        bool isRunning = _joystick.Horizontal != 0 || _joystick.Vertical != 0;

        // Передаем параметр в Animator
        _animator.SetBool("IsRunning", isRunning);
    }
}
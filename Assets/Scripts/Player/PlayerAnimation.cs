using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private FixedJoystick _joystick;

    private void Update()
    {
        // Проверяем, активен ли джойстик (есть ввод)
        bool isRunning = _joystick.Horizontal != 0 || _joystick.Vertical != 0;

        // Передаем параметр в Animator
        _animator.SetBool("IsRunning", isRunning);
    }
}
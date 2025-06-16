using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator _animator;
    public FixedJoystick _joystick;
    private bool _initialized = false;
    private PlayerShooting _playerShooting;

    // Добавляем триггеры для анимаций атаки
    [SerializeField] private string[] meleeAttackTriggers = { "MeleeAttack1", "MeleeAttack2" };
    [SerializeField] private string[] rangedAttackTriggers = { "RangedAttack1", "RangedAttack2" };

    public void Initialize(FixedJoystick joystick)
    {
        _joystick = joystick;
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        

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

        bool isRunning = (_joystick != null) &&
                        (_joystick.Horizontal != 0 || _joystick.Vertical != 0);

        if (_animator != null && _animator.isActiveAndEnabled)
        {
            _animator.SetBool("IsRunning", isRunning);
        }
    }

    // Метод для запуска случайной анимации ближней атаки
    public void TriggerMeleeAttack()
    {
        if (_animator != null && _animator.isActiveAndEnabled && meleeAttackTriggers.Length > 0)
        {
            int randomIndex = Random.Range(0, meleeAttackTriggers.Length);
            _animator.SetTrigger(meleeAttackTriggers[randomIndex]);
        }
    }

    // Метод для запуска случайной анимации дальнобойной атаки
    public void TriggerRangedAttack()
    {
        if (_animator != null && _animator.isActiveAndEnabled && rangedAttackTriggers.Length > 0)
        {
            int randomIndex = Random.Range(0, rangedAttackTriggers.Length);
            _animator.SetTrigger(rangedAttackTriggers[randomIndex]);
        }
    }
}
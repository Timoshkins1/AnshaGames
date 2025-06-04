using UnityEngine;

public abstract class PlayerAttack : MonoBehaviour
{
    [Header("Base Settings")]
    public float staminaCost = 20f;
    public float cooldown = 0.5f;
    public float maxStamina = 100f;
    public float staminaRegen = 5f;

    protected FixedJoystick _joystick;
    protected Transform _attackPoint;
    protected float _currentStamina;
    protected float _currentCooldown;
    protected bool _isAiming;

    public void Initialize(FixedJoystick joystick, Transform attackPoint)
    {
        _joystick = joystick;
        _attackPoint = attackPoint;
        _currentStamina = maxStamina;
    }

    private void Update()
    {
        HandleStamina();
        HandleAttackInput();
    }

    private void HandleStamina()
    {
        if (_currentStamina < maxStamina)
        {
            _currentStamina += staminaRegen * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, maxStamina);
        }

        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    private void HandleAttackInput()
    {
        if (_joystick == null) return;

        if (_joystick.Direction.magnitude > 0.5f)
        {
            _isAiming = true;
            Debug.DrawRay(_attackPoint.position,
                new Vector3(_joystick.Horizontal, 0, _joystick.Vertical).normalized * 5f,
                Color.red);
        }
        else if (_isAiming)
        {
            TryAttack();
            _isAiming = false;
        }
    }

    protected virtual void TryAttack()
    {
        if (_currentCooldown > 0 || _currentStamina < staminaCost) return;
        _currentStamina -= staminaCost;
        _currentCooldown = cooldown;
        ExecuteAttack();
    }

    protected abstract void ExecuteAttack();
}
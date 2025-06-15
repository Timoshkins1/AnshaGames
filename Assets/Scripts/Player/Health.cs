using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _healthIncreasePerUpgrade = 20f; // New: Health increase amount per power-up
    public float MaxHealth => _maxHealth;
    public HealthBar healthBar;
    private float _currentHealth;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;

    [ContextMenu("Test Death")]
    private void TestDeath()
    {
        TakeDamage(_maxHealth);
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(_maxHealth);
            healthBar.SetHealth(_currentHealth);
        }
    }

    public void Initialize(HealthBar healthBar)
    {
        this.healthBar = healthBar;
        healthBar.SetMaxHealth(_maxHealth);
        healthBar.SetHealth(_currentHealth);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(_currentHealth);
        }

        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        GameManager.Instance.GameOver();
        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        if (healthBar != null)
        {
            healthBar.SetHealth(_currentHealth);
        }
        OnHealthChanged?.Invoke(_currentHealth);
    }

    // New method for power-up system
    public void IncreaseMaxHealth(float amount)
    {
        _maxHealth += amount;
        _currentHealth += amount; // Also increase current health

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(_maxHealth);
            healthBar.SetHealth(_currentHealth);
        }

        OnHealthChanged?.Invoke(_currentHealth);
    }

    // Alternative version that uses predefined increase amount
    public void UpgradeMaxHealth()
    {
        IncreaseMaxHealth(_healthIncreasePerUpgrade);
    }
}
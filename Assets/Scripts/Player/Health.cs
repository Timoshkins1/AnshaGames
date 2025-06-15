using UnityEngine;
using System;
//Здоровье игрока
public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _healthIncreasePerUpgrade = 20f;
    [SerializeField] private float _healthRegenPercentPerSecond = 1f; // Новый параметр: процент восстановления в секунду

    public float MaxHealth => _maxHealth;
    public HealthBar healthBar;
    private float _currentHealth;
    private float _timeSinceLastHeal = 0f;

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

    private void Update()
    {
        // Регенерация здоровья каждую секунду
        _timeSinceLastHeal += Time.deltaTime;

        if (_timeSinceLastHeal >= 1f)
        {
            HealPercent(_healthRegenPercentPerSecond);
            _timeSinceLastHeal = 0f;
        }
    }

    // Новый метод: восстановление здоровья по проценту от максимального здоровья
    private void HealPercent(float percent)
    {
        if (_currentHealth >= _maxHealth) return;

        float healAmount = _maxHealth * (percent / 100f);
        _currentHealth += healAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(_currentHealth);
        }

        OnHealthChanged?.Invoke(_currentHealth);
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

    public void IncreaseMaxHealth(float amount)
    {
        _maxHealth += amount;
        _currentHealth += amount;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(_maxHealth);
            healthBar.SetHealth(_currentHealth);
        }

        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void UpgradeMaxHealth()
    {
        IncreaseMaxHealth(_healthIncreasePerUpgrade);
    }
}
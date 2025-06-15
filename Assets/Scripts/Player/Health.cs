using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth => _maxHealth; // ��������� public ��������
    public HealthBar healthBar; // ������ public ��� ��������� �� PlayerManager
    private float _currentHealth;


    public event Action OnDeath;
    public event Action<float> OnHealthChanged; // float - ������� ��������

    [ContextMenu("Test Death")]
    private void TestDeath()
    {
        TakeDamage(_maxHealth);
    }
    private void Start()
    {
        _currentHealth = _maxHealth;
        healthBar.SetMaxHealth(_currentHealth); 
    }
    public void Initialize(HealthBar healthBar)
    {
        this.healthBar = healthBar;
        healthBar.SetMaxHealth(_maxHealth);
    }
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        healthBar.SetHealth(_currentHealth);
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
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
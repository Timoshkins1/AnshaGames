using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged; // float - текущее здоровье

    [ContextMenu("Test Death")]
    private void TestDeath()
    {
        TakeDamage(_maxHealth);
    }
    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
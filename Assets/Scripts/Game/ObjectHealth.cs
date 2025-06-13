using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    // Start is called before the first frame update
     [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public event System.Action OnDeath;
    public event System.Action<int> OnDamageTaken;

    public void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnDamageTaken?.Invoke(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}

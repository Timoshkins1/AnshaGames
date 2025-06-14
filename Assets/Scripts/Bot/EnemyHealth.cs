using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    private int currentHealth;
    private Camera mainCamera;
    private BrawlStarsBotAI botAI;

    public event System.Action OnDeath;

    public int MaxHealth => maxHealth;

    private void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        botAI = GetComponent<BrawlStarsBotAI>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        UpdateHealthText();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthSlider != null) healthSlider.value = currentHealth;
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        foreach (var collider in GetComponents<Collider>())
            collider.enabled = false;

        if (healthCanvas != null)
            healthCanvas.enabled = false;

        if (botAI != null)
        {
            botAI.HandleDeath();
        }
        else
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
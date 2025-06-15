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

    public enum CanvasLookDirection
    {
        WorldForward,   // �� ������� ���� (Z+)
        WorldUp,        // ����� (Y+)
        WorldRight,    // ������ (X+)
        CustomRotation // ������ �������
    }

    [SerializeField] private CanvasLookDirection canvasLookDirection = CanvasLookDirection.WorldForward;
    [SerializeField] private Vector3 customRotationEuler = Vector3.zero; // ������������ ������ ���� ������ CustomRotation

    private float currentHealth;
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

        // ������������� ������� � �������� �������
        UpdateCanvasPositionAndRotation();
    }

    private void LateUpdate()
    {
        UpdateCanvasPositionAndRotation();
    }

    private void UpdateCanvasPositionAndRotation()
    {
        if (healthCanvas == null) return;

        // ��������� ������� (����� ��������� �� �����)
        healthCanvas.transform.position = transform.position + healthBarOffset;

        // ������������� ������� � ����������� �� ���������� ������
        switch (canvasLookDirection)
        {
            case CanvasLookDirection.WorldForward:
                healthCanvas.transform.rotation = Quaternion.identity; // ������� �� ��� Z+
                break;
            case CanvasLookDirection.WorldUp:
                healthCanvas.transform.rotation = Quaternion.Euler(90f, 0, 0); // ������� ����� (Y+)
                break;
            case CanvasLookDirection.WorldRight:
                healthCanvas.transform.rotation = Quaternion.Euler(0, 90f, 0); // ������� ������ (X+)
                break;
            case CanvasLookDirection.CustomRotation:
                healthCanvas.transform.rotation = Quaternion.Euler(customRotationEuler);
                break;
        }
    }

    // ��������� ��� ��� ���������...
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

    public void TakeDamage(float damage)
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
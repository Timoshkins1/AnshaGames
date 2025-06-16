using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject powerUpPanel;
    [SerializeField] private Transform powerUpContainer;
    [SerializeField] private PowerUpOption powerUpOptionPrefab;


    [Header("Power Up Settings")]
    [SerializeField] private PowerUpData[] allPowerUps;

    private List<PowerUpData> currentOptions = new List<PowerUpData>();
    private bool waitingForSelection = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        powerUpPanel.SetActive(false);
    }

    public void ShowPowerUpSelection()
    {
        if (waitingForSelection) return;

        // Select 3 random unique power-ups
        currentOptions.Clear();
        var availablePowerUps = new List<PowerUpData>(allPowerUps);

        for (int i = 0; i < 3 && availablePowerUps.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availablePowerUps.Count);
            currentOptions.Add(availablePowerUps[randomIndex]);
            availablePowerUps.RemoveAt(randomIndex);
        }

        // Create UI options
        ClearPowerUpOptions();
        foreach (var powerUp in currentOptions)
        {
            var option = Instantiate(powerUpOptionPrefab, powerUpContainer);
            option.Initialize(powerUp, this);
        }

        powerUpPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
        waitingForSelection = true;
    }

    public void SelectPowerUp(PowerUpData powerUp)
    {
        if (!waitingForSelection) return;

        ApplyPowerUp(powerUp);
        ClearPowerUpOptions();
        powerUpPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
        waitingForSelection = false;

        // Обновляем UI после применения улучшения
        UpdatePlayerUI();
    }

    private void UpdatePlayerUI()
    {
        var player = PlayerManager.Instance.PlayerTransform.gameObject;
        var shooting = player.GetComponent<PlayerShooting>();
        if (shooting != null)
        {
            var ammoDisplay = FindObjectOfType<AmmoDisplay>();
            if (ammoDisplay != null)
            {
                ammoDisplay.Initialize(shooting.attackConfig.maxAmmo);
            }
        }
    }
    private void ApplyPowerUp(PowerUpData powerUp)
    {
        var player = PlayerManager.Instance.PlayerTransform.gameObject;

        switch (powerUp.powerUpType)
        {
            case PowerUpType.Health:
                var health = player.GetComponent<Health>();
                if (health != null) health.IncreaseMaxHealth(powerUp.value);
                break;

            case PowerUpType.Damage:
                var shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null) shooting.attackConfig.damagePerBullet += (int)powerUp.value;
                break;

            case PowerUpType.MoveSpeed:
                var controller = player.GetComponent<PlayerController>();
                if (controller != null) controller.IncreaseMoveSpeed(powerUp.value);
                break;

            case PowerUpType.BushSpeed:
                controller = player.GetComponent<PlayerController>();
                if (controller != null) controller.IncreaseBushMultiplier(powerUp.value);
                break;

            case PowerUpType.BushFade:
                controller = player.GetComponent<PlayerController>();
                if (controller != null) controller.IncreaseBoostFadeDuration(powerUp.value);
                break;

            case PowerUpType.AmmoCapacity:
                shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null) shooting.attackConfig.maxAmmo += (int)powerUp.value;
                break;

            case PowerUpType.ReloadSpeed:
                shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null) shooting.attackConfig.reloadTime -= powerUp.value;
                break;
        }
    }

    private void ClearPowerUpOptions()
    {
        foreach (Transform child in powerUpContainer)
        {
            Destroy(child.gameObject);
        }
    }
}

public enum PowerUpType
{
    Health,
    Damage,
    MoveSpeed,
    BushSpeed,
    BushFade,
    AmmoCapacity,
    ReloadSpeed
}

[System.Serializable]
public class PowerUpData
{
    public PowerUpType powerUpType;
    public string displayName;
    public string description;
    public Sprite icon;
    public float value;
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpOption : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button selectButton;

    private PowerUpData powerUpData;
    private PowerUpManager powerUpManager;

    public void Initialize(PowerUpData data, PowerUpManager manager)
    {
        powerUpData = data;
        powerUpManager = manager;

        iconImage.sprite = data.icon;
        nameText.text = data.displayName;
        descriptionText.text = data.description;

        selectButton.onClick.AddListener(OnSelect);
    }

    private void OnSelect()
    {
        powerUpManager.SelectPowerUp(powerUpData);
    }
}
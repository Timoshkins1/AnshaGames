using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private Sprite playerSprite;
    [SerializeField] private string playerName = "Player";

    public Sprite GetPlayerSprite() => playerSprite;
    public string GetPlayerName() => playerName;
}
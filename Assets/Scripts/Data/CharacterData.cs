using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public int characterID; // ”никальный ID персонажа
    public string characterName;
    [TextArea] public string description;
    public string attackType;
    public string attackTypeID;
    public string damage;
    public string health;
    public string speed;
    public GameObject characterPrefab;

    [Header("Spawn Settings")]
    public Vector3 spawnPositionOffset = Vector3.zero;
    public Vector3 spawnRotation = Vector3.zero;
}

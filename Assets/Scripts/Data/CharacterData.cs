using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    [TextArea] public string description;
    public string attackType;
    public int damage;
    public int health;
    public int speed;
    public GameObject characterPrefab;

    [Header("Spawn Settings")]
    public Vector3 spawnPositionOffset = Vector3.zero;
    public Vector3 spawnRotation = Vector3.zero;
}
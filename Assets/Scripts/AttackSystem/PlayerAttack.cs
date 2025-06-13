using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerAttack", menuName = "Game/Player Attack Config")]
public class PlayerAttack : ScriptableObject
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public int bulletsCount = 5;
    public float spreadAngle = 30f;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 5f;

    [Header("Damage Settings")]
    public int damagePerBullet = 10;
    public float knockbackForce = 5f;
}
using UnityEngine;

public class ShotgunAttack : PlayerAttack
{
    [Header("Shotgun Settings")]
    public GameObject projectilePrefab;
    public int pellets = 5;
    public float spreadAngle = 30f;
    public float projectileSpeed = 15f;
    public int damagePerPellet = 5;

    protected override void ExecuteAttack()
    {
        Vector3 baseDirection = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical).normalized;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = Quaternion.Euler(
                0,
                Random.Range(-spreadAngle, spreadAngle),
                0) * baseDirection;

            GameObject pellet = Instantiate(projectilePrefab, _attackPoint.position, Quaternion.LookRotation(direction));
            pellet.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
            pellet.GetComponent<Projectile>().SetDamage(damagePerPellet);
        }
    }
}
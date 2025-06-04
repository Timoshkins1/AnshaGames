using UnityEngine;

public class MeleeAttack : PlayerAttack
{
    [Header("Melee Settings")]
    public float range = 2f;
    public float angle = 90f;
    public int damage = 15;
    public LayerMask enemyLayer;

    protected override void ExecuteAttack()
    {
        Collider[] hits = Physics.OverlapSphere(_attackPoint.position, range, enemyLayer);

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - _attackPoint.position).normalized;
            if (Vector3.Angle(_attackPoint.forward, dirToTarget) < angle / 2)
            {
                if (hit.TryGetComponent<Health>(out var health))
                {
                    health.TakeDamage(damage);
                }
            }
        }
    }
}
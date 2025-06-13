using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float detectRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject collectEffect;

    private bool collected = false;

    private void Update()
    {
        if (collected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        GameManager.Instance.DiamondCollected();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float detectRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject collectEffect;

    private bool collected = false;

    public void ResetDiamond()
    {
        collected = false;
    }

    private void Update()
    {
        if (collected) return;

        if (Physics.CheckSphere(transform.position, detectRadius, playerLayer))
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
        gameObject.SetActive(false); // Вместо Destroy
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float detectRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject collectEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            DiamondCarrier carrier = other.GetComponent<DiamondCarrier>();
            if (carrier != null)
            {
                Collect(carrier);
            }
        }
    }

    private void Collect(DiamondCarrier carrier)
    {
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        carrier.CollectDiamond();
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }

    public void ResetDiamond()
    {
        // Метод для сброса состояния при возврате в пул
    }
}
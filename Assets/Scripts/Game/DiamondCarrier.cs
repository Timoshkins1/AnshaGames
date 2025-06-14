using UnityEngine;

public class DiamondCarrier : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float baseDetectionRadius = 3f;
    [SerializeField] private LayerMask baseLayer;
    [SerializeField] private float depositCooldown = 0.5f;

    private int carriedDiamonds = 0;
    private float lastDepositTime;
    private bool isNearBase = false;

    private void Update()
    {
        isNearBase = Physics.CheckSphere(transform.position, baseDetectionRadius, baseLayer);

        // Автоматически сбрасываем алмазы при нахождении на базе
        if (isNearBase && carriedDiamonds > 0 && Time.time > lastDepositTime + depositCooldown)
        {
            DepositDiamonds();
        }
    }

    public void CollectDiamond()
    {
        carriedDiamonds++;
        Debug.Log($"Collected diamond. Total: {carriedDiamonds}");
    }

    private void DepositDiamonds()
    {
        lastDepositTime = Time.time;
        GameManager.Instance.DiamondCollected();
        carriedDiamonds--;
        Debug.Log($"Deposited 1 diamond. Remaining: {carriedDiamonds}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isNearBase ? Color.green : Color.blue;
        Gizmos.DrawWireSphere(transform.position, baseDetectionRadius);
    }
}
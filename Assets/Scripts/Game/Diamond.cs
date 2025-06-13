using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float detectRadius = 2f; // Расстояние обнаружения игрока
    [SerializeField] private LayerMask playerLayer;    // Слой игрока для проверки

    private bool collected = false;

    private void Update()
    {
        if (collected) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            // Игрок найден — собираем алмаз
            Collect();
        }
    }

    private void Collect()
    {
        collected = true;
        // Можно добавить эффект сбора (звук, анимацию и т.д.)
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
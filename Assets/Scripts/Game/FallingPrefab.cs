using UnityEngine;

public class FallingPrefab : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallDelay = 3f; // Задержка перед падением
    [SerializeField] private float fallSpeed = 5f; // Скорость падения
    [SerializeField] private float destroyDelayAfterFall = 1f; // Задержка перед уничтожением после начала падения

    private Rigidbody rb;
    private bool isFalling = false;
    private float fallStartTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Запускаем таймер падения
        Invoke(nameof(StartFalling), fallDelay);
    }

    private void StartFalling()
    {
        isFalling = true;
        fallStartTime = Time.time;

        // Если есть Rigidbody, отключаем использование гравитации (будем управлять вручную)
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (isFalling)
        {
            // Двигаем объект вниз
            if (rb != null)
            {
                rb.velocity = Vector3.down * fallSpeed;
            }
            else
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            }

            // Проверяем, не пора ли уничтожить объект
            if (Time.time - fallStartTime >= destroyDelayAfterFall)
            {
                Destroy(gameObject);
            }
        }
    }
}
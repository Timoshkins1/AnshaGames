using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Ссылка на трансформ игрока
    private Vector3 offset; // Смещение камеры относительно игрока

    void Start()
    {
        // Вычисляем начальное смещение камеры относительно игрока
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Обновляем позицию камеры с учетом смещения
            transform.position = player.position + offset;
        }
    }
}
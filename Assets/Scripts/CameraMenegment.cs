using UnityEngine;

public class CameraMenegment : MonoBehaviour
{
    public Transform Player; // —сылка на трансформ игрока (назнаетс€ извне)
    private Vector3 offset; // —мещение камеры относительно игрока

    void Start()
    {
        // ¬ычисл€ем начальное смещение камеры относительно игрока
        if (Player != null)
        {
            offset = transform.position - Player.position;
        }
        else
        {
            Debug.LogError("CameraFollow: Player transform не назначен!");
        }
    }

    void LateUpdate()
    {
        if (Player != null)
        {
            // ќбновл€ем позицию камеры с учетом смещени€
            transform.position = Player.position + offset;
        }
    }
}
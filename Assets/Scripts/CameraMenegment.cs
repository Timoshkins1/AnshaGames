using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // ������ �� ��������� ������
    private Vector3 offset; // �������� ������ ������������ ������

    void Start()
    {
        // ��������� ��������� �������� ������ ������������ ������
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // ��������� ������� ������ � ������ ��������
            transform.position = player.position + offset;
        }
    }
}
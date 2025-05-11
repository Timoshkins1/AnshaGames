using UnityEngine;

public class CameraMenegment : MonoBehaviour
{
    public Transform Player; // ������ �� ��������� ������ (��������� �����)
    private Vector3 offset; // �������� ������ ������������ ������

    void Start()
    {
        // ��������� ��������� �������� ������ ������������ ������
        if (Player != null)
        {
            offset = transform.position - Player.position;
        }
        else
        {
            Debug.LogError("CameraFollow: Player transform �� ��������!");
        }
    }

    void LateUpdate()
    {
        if (Player != null)
        {
            // ��������� ������� ������ � ������ ��������
            transform.position = Player.position + offset;
        }
    }
}
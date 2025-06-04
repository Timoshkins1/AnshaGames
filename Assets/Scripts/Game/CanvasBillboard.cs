using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main; // �������� ������� ������
    }

    private void LateUpdate() // ����� ������������ LateUpdate, ����� �������� �������
    {
        if (_mainCamera == null) return;

        // ������������ Canvas � ������ (�� ��� ������� �����/����, ���� �����)
        transform.rotation = _mainCamera.transform.rotation;

        // ������������: ������ � ������, �� �������� ��������� (���� �����)
        //transform.LookAt(transform.position + _mainCamera.transform.forward, Vector3.up);
    }
}
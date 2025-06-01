using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraPivot; // ������, � �������� ��������� ������
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Vector3 _defaultRotation = new Vector3(0, 0, 0); // ��������� �������
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -45, 0); // ������� ��� 2-� ������

    private bool _shouldRotate = false;
    private Vector3 _desiredRotation;

    private void Start()
    {
        _desiredRotation = _defaultRotation;
        _cameraPivot.rotation = Quaternion.Euler(_defaultRotation);
    }

    private void Update()
    {
        if (_shouldRotate)
        {
            // ������� ������� ������
            _cameraPivot.rotation = Quaternion.Lerp(
                _cameraPivot.rotation,
                Quaternion.Euler(_desiredRotation),
                _rotationSpeed * Time.deltaTime
            );

            // ��������� ��� ���������� ����
            if (Quaternion.Angle(_cameraPivot.rotation, Quaternion.Euler(_desiredRotation)) < 0.1f)
            {
                _shouldRotate = false;
            }
        }
    }

    // ���������� ��� ������� ������ "������"
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    // ���������� ��� ������� ������ "��������� ������"
    public void RotateCamera()
    {
        _desiredRotation = _targetRotation;
        _shouldRotate = true;
    }

    // ���������� ��� ������� ������ "�������� ������"
    public void ResetCameraRotation()
    {
        _desiredRotation = _defaultRotation;
        _shouldRotate = true;
    }
}
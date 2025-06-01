using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraPivot; // Объект, к которому привязана камера
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Vector3 _defaultRotation = new Vector3(0, 0, 0); // Стартовый поворот
    [SerializeField] private Vector3 _targetRotation = new Vector3(0, -45, 0); // Поворот для 2-й кнопки

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
            // Плавный поворот камеры
            _cameraPivot.rotation = Quaternion.Lerp(
                _cameraPivot.rotation,
                Quaternion.Euler(_desiredRotation),
                _rotationSpeed * Time.deltaTime
            );

            // Остановка при достижении цели
            if (Quaternion.Angle(_cameraPivot.rotation, Quaternion.Euler(_desiredRotation)) < 0.1f)
            {
                _shouldRotate = false;
            }
        }
    }

    // Вызывается при нажатии кнопки "Играть"
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Вызывается при нажатии кнопки "Повернуть камеру"
    public void RotateCamera()
    {
        _desiredRotation = _targetRotation;
        _shouldRotate = true;
    }

    // Вызывается при нажатии кнопки "Сбросить камеру"
    public void ResetCameraRotation()
    {
        _desiredRotation = _defaultRotation;
        _shouldRotate = true;
    }
}
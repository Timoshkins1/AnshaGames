using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target; // Игрок (передаем ссылку вручную!)

    [Header("Offset")]
    [SerializeField] private Vector3 _positionOffset = new Vector3(2f, 3f, -4f); // X смещен, Y выше, Z назад
    [SerializeField] private float _rotationX = 30f; // Наклон камеры в градусах

    [Header("Smoothing")]
    [SerializeField] private float _followSpeed = 5f; // Плавность слежения

    private void LateUpdate()
    {
        if (_target == null) return;

        // Позиция камеры с учетом смещения
        Vector3 targetPosition = _target.position + _positionOffset;

        // Плавное перемещение
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

        // Наклон камеры
        transform.rotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }
}
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _positionOffset = new Vector3(2f, 3f, -4f);
    [SerializeField] private float _rotationX = 30f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _targetHeightOffset = 1f;

    private Transform _target;
    private Vector3 _smoothedPosition;
    private Vector3 _originalPosition;
    private bool _isShaking = false;
    private float _shakeDuration;
    private float _shakeAmplitude;
    private float _shakeTimer;

    public void SetPlayerTransform(Transform playerTransform)
    {
        _target = playerTransform;

        if (_target == null)
        {
            Debug.LogError("CameraFollow: Invalid player transform assigned!");
            enabled = false;
            return;
        }

        // Первоначальная позиция без Lerp
        if (_target != null)
        {
            _smoothedPosition = CalculateTargetPosition();
            transform.position = _smoothedPosition;
            transform.rotation = Quaternion.Euler(_rotationX, 0f, 0f);
        }

        enabled = true;
    }

    private void LateUpdate()
    {
        

        if (!IsValidTarget()) return;

        _smoothedPosition = Vector3.Lerp(
            transform.position,
            CalculateTargetPosition(),
            _followSpeed * Time.deltaTime
        );

        transform.position = _smoothedPosition;
        transform.rotation = Quaternion.Euler(_rotationX, 0f, 0f);

        // Обработка тряски
        if (_isShaking)
        {
            _shakeTimer -= Time.deltaTime;

            if (_shakeTimer <= 0f)
            {
                _isShaking = false;
                transform.localPosition = _smoothedPosition;
            }
            else
            {
                // Добавляем случайное смещение к позиции камеры
                Vector3 shakeOffset = Random.insideUnitSphere * _shakeAmplitude;
                transform.position = _smoothedPosition + shakeOffset;
            }
        }
    }

    /// <summary>
    /// Запускает эффект тряски камеры
    /// </summary>
    /// <param name="amplitude">Амплитуда тряски</param>
    /// <param name="duration">Длительность тряски в секундах</param>
    public void Shake(float amplitude, float duration)
    {
        if (_isShaking) return; // Уже трясется

        _isShaking = true;
        _shakeAmplitude = amplitude;
        _shakeDuration = duration;
        _shakeTimer = duration;
    }

    private Vector3 CalculateTargetPosition()
    {
        return _target.position +
               Vector3.up * _targetHeightOffset +
               _positionOffset;
    }

    private bool IsValidTarget()
    {
        return _target != null &&
               _target.gameObject.activeInHierarchy &&
               _target.gameObject.scene.isLoaded;
    }

    // Для отладки в редакторе
    private void OnDrawGizmosSelected()
    {
        if (_target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, CalculateTargetPosition());
            Gizmos.DrawWireSphere(CalculateTargetPosition(), 0.5f);
        }
    }
}
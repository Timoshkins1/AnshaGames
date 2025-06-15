using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _positionOffset = new Vector3(2f, 3f, -4f);
    [SerializeField] private float _rotationX = 30f;
    [SerializeField] private float _followSpeed = 5f;
    [SerializeField] private float _targetHeightOffset = 1f; // Новый параметр

    private Transform _target;
    private Vector3 _smoothedPosition;

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
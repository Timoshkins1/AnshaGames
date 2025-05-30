using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Settings")]
    [SerializeField] private Vector3 _positionOffset = new Vector3(2f, 3f, -4f);
    [SerializeField] private float _rotationX = 30f;
    [SerializeField] private float _followSpeed = 5f;

    private void LateUpdate()
    {
        if (_target == null || !_target.gameObject.activeInHierarchy) return;

        Vector3 targetPosition = _target.position + _positionOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }
}
using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main; // Получаем главную камеру
    }

    private void LateUpdate() // Лучше использовать LateUpdate, чтобы избежать дёрганий
    {
        if (_mainCamera == null) return;

        // Поворачиваем Canvas к камере (но без наклона вверх/вниз, если нужно)
        transform.rotation = _mainCamera.transform.rotation;

        // Альтернатива: строго в камеру, но сохраняя вертикаль (если нужно)
        //transform.LookAt(transform.position + _mainCamera.transform.forward, Vector3.up);
    }
}
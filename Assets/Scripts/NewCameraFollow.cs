using UnityEngine;
using Photon.Pun;

public class NewCameraFollow : MonoBehaviourPun
{
    [Header("Настройки")]
    public float distance = 3.5f;
    public float height = 1.7f;
    public float angle = 10f;
    public float smoothness = 5f;

    private Transform _player;

    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        _player = transform.parent;
        ResetCameraPosition();
    }

    void LateUpdate()
    {
        if (!photonView.IsMine || _player == null) return;

        Vector3 targetPos = _player.position +
                          -_player.forward * distance +
                          _player.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothness * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(angle, _player.eulerAngles.y, 0),
            smoothness * Time.deltaTime
        );
    }

    void ResetCameraPosition()
    {
        transform.localPosition = new Vector3(0, height, -distance);
        transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }
}
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f;
    public Joystick joystick; // Джойстик из Canvas
    public Transform firePoint;
    public GameObject bulletPrefab;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!photonView.IsMine) // Отключаем управление, если это не наш игрок
        {
            Destroy(GetComponent<PlayerController>());
            return;
        }

        // Находим джойстик в сцене (если он не в префабе)
        joystick = FindObjectOfType<Joystick>();
        Camera.main.GetComponent<CameraFollow>().target = transform; // Привязываем камеру
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Движение через джойстик
        Vector2 moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        rb.velocity = moveInput * moveSpeed;

        // Стрельба по тапу (или другой кнопке)
        if (Input.GetMouseButtonDown(0))
        {
            photonView.RPC("Shoot", RpcTarget.All);
        }
    }

    [PunRPC]
    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Синхронизация позиции, если нужно
    }
}
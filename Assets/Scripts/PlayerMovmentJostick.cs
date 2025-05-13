using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f;
    public Joystick joystick; // �������� �� Canvas
    public Transform firePoint;
    public GameObject bulletPrefab;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!photonView.IsMine) // ��������� ����������, ���� ��� �� ��� �����
        {
            Destroy(GetComponent<PlayerController>());
            return;
        }

        // ������� �������� � ����� (���� �� �� � �������)
        joystick = FindObjectOfType<Joystick>();
        Camera.main.GetComponent<CameraFollow>().target = transform; // ����������� ������
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // �������� ����� ��������
        Vector2 moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        rb.velocity = moveInput * moveSpeed;

        // �������� �� ���� (��� ������ ������)
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
        // ������������� �������, ���� �����
    }
}
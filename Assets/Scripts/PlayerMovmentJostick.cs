using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    [SerializeField] public Joystick joystick; // SerializeField ��� ������� ���������� ��� ������
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;
    public Animator anim;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // ���� �������� �� ����� � ���������� � ��� ��������� ����� � ���� ���
        if (joystick == null && photonView.IsMine)
        {
            joystick = FindObjectOfType<Joystick>();
            if (joystick == null)
                Debug.LogError("Joystick not found!");
        }
    }

    void Update()
    {
        // �������� ������ � ��������� �������
        if (!photonView.IsMine)
            return;

        if (joystick == null)
            return;

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        bool isMoving = movement.magnitude > 0.1f;
        anim.SetBool("Run", isMoving);

        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement);
            toRotation *= Quaternion.Euler(0, -90, 0); // ������������� ��������
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
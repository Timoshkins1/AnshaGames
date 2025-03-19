using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Joystick joystick; // ������ �� ��������
    public float moveSpeed = 5f; // �������� ��������
    public float rotationSpeed = 10f; // �������� ��������

    private CharacterController characterController;
    public Animator anim; // ������ �� ��������� Animator

    void Start()
    {
        // �������� ��������� CharacterController
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // �������� ���� � ���������
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // ������� ������ ��������
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // ����������� ������, ����� ����� �� �������� ������� �� ���������
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // ���������� ������
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        // ���������, ���� �� ���� �� ���������
        bool isMoving = movement.magnitude > 0.1f; // ����� ��� ����������� ��������
        anim.SetBool("Run", isMoving); // ������������� �������� "Run" � Animator

        // ������������ ������ � ����������� ��������
        if (isMoving)
        {
            // ������������ ����������� ��������
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);

            // ������������ ������� �� 90 �������� �� ��� Y, ����� ��������� ���������� ������
            toRotation *= Quaternion.Euler(0, -90, 0); // ������� �� -90 �������� �� ��� Y

            // ������ ������������ ���������
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
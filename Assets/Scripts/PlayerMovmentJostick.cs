using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Joystick joystick; // Ссылка на джойстик
    public float moveSpeed = 5f; // Скорость движения
    public float rotationSpeed = 10f; // Скорость поворота

    private CharacterController characterController;
    public Animator anim; // Ссылка на компонент Animator

    void Start()
    {
        // Получаем компонент CharacterController
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Получаем ввод с джойстика
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Создаем вектор движения
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // Нормализуем вектор, чтобы игрок не двигался быстрее по диагонали
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Перемещаем игрока
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        // Проверяем, есть ли ввод от джойстика
        bool isMoving = movement.magnitude > 0.1f; // Порог для определения движения
        anim.SetBool("Run", isMoving); // Устанавливаем параметр "Run" в Animator

        // Поворачиваем игрока в направлении движения
        if (isMoving)
        {
            // Рассчитываем направление поворота
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);

            // Корректируем поворот на 90 градусов по оси Y, чтобы исправить ориентацию модели
            toRotation *= Quaternion.Euler(0, -90, 0); // Поворот на -90 градусов по оси Y

            // Плавно поворачиваем персонажа
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
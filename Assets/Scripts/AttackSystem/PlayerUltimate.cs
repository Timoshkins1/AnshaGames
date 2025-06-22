using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerUltimate : MonoBehaviour
{
    [SerializeField] protected Slider ultimateChargeSlider;
    [SerializeField] protected float maxCharge = 100f;
    [SerializeField] protected float joystickDeadZone = 0.3f;
    protected float currentCharge = 0f;
    protected Joystick ultimateJoystick;
    protected Vector3 ultimateDirection = Vector3.forward;
    protected bool isUltimateJoystickActive = false;

    public bool IsUltimateReady() => currentCharge >= maxCharge;

    public void ResetCharge()
    {
        currentCharge = 0f;
        UpdateSlider();
    }

    public void Initialize(Slider slider, Joystick joystick)
    {
        ultimateChargeSlider = slider;
        ultimateJoystick = joystick;
        UpdateSlider();
    }

    public void AddCharge(float amount)
    {
        currentCharge = Mathf.Min(currentCharge + amount, maxCharge);
        UpdateSlider();
    }

    protected void UpdateSlider()
    {
        ultimateChargeSlider?.SetValueWithoutNotify(currentCharge / maxCharge);
    }

    private void Update()
    {
        if (!IsUltimateReady() || ultimateJoystick == null) return;

        Vector2 joystickInput = new Vector2(ultimateJoystick.Horizontal, ultimateJoystick.Vertical);
        float inputMagnitude = joystickInput.magnitude;

        // ���� �������� ������ ��� �������������
        if (inputMagnitude > joystickDeadZone && !isUltimateJoystickActive)
        {
            isUltimateJoystickActive = true;
        }

        // ���� �������� �������, ��������� �����������
        if (isUltimateJoystickActive)
        {
            if (inputMagnitude > joystickDeadZone)
            {
                ultimateDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;
            }
            else if (inputMagnitude <= joystickDeadZone && isUltimateJoystickActive)
            {
                // ���������� ����� ��� ���������� ���������
                ActivateUltimateWithDirection();
                ResetCharge();
                isUltimateJoystickActive = false;
            }
        }
    }

    private void ActivateUltimateWithDirection()
    {
        // ������������ ������ ������ � ����������� �����
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.HandleAttackRotation(ultimateDirection);
        }

        // ���������� �����
        ActivateUltimate();
    }

    public abstract void ActivateUltimate();
}
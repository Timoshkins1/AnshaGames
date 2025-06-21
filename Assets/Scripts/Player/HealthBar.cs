using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private CameraFollow cameraFollow;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        if (cameraFollow != null)
        {
            cameraFollow.GetComponent<CameraFollow>().Shake(0.05f, 0.1f);
        }
        slider.value = health;
    }
}
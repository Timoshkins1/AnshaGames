using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private Image[] ammoIcons; // Массив иконок патронов
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color reloadingColor = Color.gray;
    [SerializeField] private Image reloadProgressBar; // Полоса перезарядки

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        // Обновляем иконки патронов
        for (int i = 0; i < ammoIcons.Length; i++)
        {
            if (i < maxAmmo)
            {
                ammoIcons[i].gameObject.SetActive(true);
                ammoIcons[i].color = i < currentAmmo ? activeColor : reloadingColor;
            }
            else
            {
                ammoIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateReloadProgress(float progress)
    {
        if (reloadProgressBar != null)
        {
            reloadProgressBar.fillAmount = progress;
        }
    }
}
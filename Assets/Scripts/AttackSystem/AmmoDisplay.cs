using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private Image[] ammoIcons; // ������ ������ ��������
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color reloadingColor = Color.gray;
    [SerializeField] private Image reloadProgressBar; // ������ �����������

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        // ��������� ������ ��������
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
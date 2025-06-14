using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject _ammoIconPrefab;
    [SerializeField] private Transform _iconsContainer;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _emptyColor = Color.gray;
    [SerializeField] private Color _reloadingColor = Color.yellow; // Цвет иконки при перезарядке

    private Image[] _ammoIcons;
    private bool _isReloading;

    public void Initialize(int maxAmmo)
    {
        foreach (Transform child in _iconsContainer)
        {
            Destroy(child.gameObject);
        }

        _ammoIcons = new Image[maxAmmo];
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject icon = Instantiate(_ammoIconPrefab, _iconsContainer);
            _ammoIcons[i] = icon.GetComponent<Image>();
            _ammoIcons[i].color = _activeColor;
        }
    }

    public void UpdateAmmo(int currentAmmo)
    {
        for (int i = 0; i < _ammoIcons.Length; i++)
        {
            if (_isReloading && i == currentAmmo)
            {
                // Текущий патрон, который перезаряжается
                _ammoIcons[i].color = _reloadingColor;
            }
            else
            {
                _ammoIcons[i].color = i < currentAmmo ? _activeColor : _emptyColor;
            }
        }
    }

    public void SetReloadingState(bool isReloading)
    {
        _isReloading = isReloading;
    }
}
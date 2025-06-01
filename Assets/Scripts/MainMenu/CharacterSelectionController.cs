using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class CharacterSelectionController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _attackTypeText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private GameObject _confirmButton;

    [Header("Animation Settings")]
    [SerializeField] private Transform _modelContainer;
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _hideYPosition = -10f;
    [SerializeField] private float _showYPosition = 0f;

    [Header("Character List")]
    [SerializeField] private List<CharacterData> _characters = new List<CharacterData>();

    private GameObject _currentModel;
    private int _currentIndex = 0;
    private bool _isAnimating = false;
    private Vector3 _originalContainerPosition;
    private int _selectedCharacterID = -1;

    private const string FIRST_RUN_KEY = "FirstRun";
    private const int DEFAULT_CHARACTER_ID = 1;
    private const string SELECTED_CHARACTER_KEY = "SelectedCharacterID";

    private void Start()
    {
        _originalContainerPosition = _modelContainer.position;
        _modelContainer.position = new Vector3(
            _originalContainerPosition.x,
            _showYPosition,
            _originalContainerPosition.z
        );

        // Проверяем первый ли это запуск
        if (!PlayerPrefs.HasKey(FIRST_RUN_KEY))
        {
            PlayerPrefs.SetInt(FIRST_RUN_KEY, 1);
            PlayerPrefs.SetInt(SELECTED_CHARACTER_KEY, DEFAULT_CHARACTER_ID);
            PlayerPrefs.Save();
        }

        // Загружаем последний выбранный персонаж
        _selectedCharacterID = PlayerPrefs.GetInt(SELECTED_CHARACTER_KEY, DEFAULT_CHARACTER_ID);

        // Находим индекс по ID или используем 0
        _currentIndex = _characters.FindIndex(c => c.characterID == _selectedCharacterID);

        // Если персонаж с таким ID не найден, используем первого в списке
        if (_currentIndex < 0)
        {
            _currentIndex = 0;
            _selectedCharacterID = _characters[0].characterID;
        }

        ShowCharacter(_currentIndex);
        UpdateConfirmButton();
    }

    public void NextCharacter()
    {
        if (_isAnimating || _characters.Count == 0) return;
        int newIndex = (_currentIndex + 1) % _characters.Count;
        StartCoroutine(SwitchCharacterCoroutine(newIndex));
    }

    public void PreviousCharacter()
    {
        if (_isAnimating || _characters.Count == 0) return;
        int newIndex = (_currentIndex - 1 + _characters.Count) % _characters.Count;
        StartCoroutine(SwitchCharacterCoroutine(newIndex));
    }

    public void ConfirmSelection()
    {
        if (_characters.Count == 0 || _currentIndex < 0 || _currentIndex >= _characters.Count)
            return;

        // Сохраняем ID выбранного персонажа
        _selectedCharacterID = _characters[_currentIndex].characterID;
        PlayerPrefs.SetInt(SELECTED_CHARACTER_KEY, _selectedCharacterID);
        PlayerPrefs.Save();

        UpdateConfirmButton();
        Debug.Log($"Выбран персонаж ID: {_selectedCharacterID}");
    }

    private IEnumerator SwitchCharacterCoroutine(int newIndex)
    {
        _isAnimating = true;

        yield return StartCoroutine(MoveModel(false));

        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        _currentIndex = newIndex;

        if (_characters[_currentIndex].characterPrefab != null)
        {
            _currentModel = Instantiate(
                _characters[_currentIndex].characterPrefab,
                _modelContainer
            );

            _currentModel.transform.localPosition = _characters[_currentIndex].spawnPositionOffset;
            _currentModel.transform.localRotation = Quaternion.Euler(_characters[_currentIndex].spawnRotation);
        }

        yield return StartCoroutine(MoveModel(true));

        UpdateCharacterInfo();
        UpdateConfirmButton();
        _isAnimating = false;
    }

    private void UpdateConfirmButton()
    {
        if (_confirmButton == null) return;

        bool isSelected = _characters[_currentIndex].characterID == _selectedCharacterID;
        _confirmButton.SetActive(!isSelected);
    }

    private IEnumerator MoveModel(bool moveUp)
    {
        float elapsed = 0;
        Vector3 startPos = _modelContainer.position;
        Vector3 endPos = moveUp
            ? new Vector3(_originalContainerPosition.x, _showYPosition, _originalContainerPosition.z)
            : new Vector3(_originalContainerPosition.x, _hideYPosition, _originalContainerPosition.z);

        while (elapsed < _animationDuration)
        {
            _modelContainer.position = Vector3.Lerp(startPos, endPos, elapsed / _animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _modelContainer.position = endPos;
    }

    private void ShowCharacter(int index)
    {
        if (_characters.Count == 0) return;

        _currentIndex = Mathf.Clamp(index, 0, _characters.Count - 1);

        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        if (_characters[_currentIndex].characterPrefab != null)
        {
            _currentModel = Instantiate(
                _characters[_currentIndex].characterPrefab,
                _modelContainer
            );

            _currentModel.transform.localPosition = _characters[_currentIndex].spawnPositionOffset;
            _currentModel.transform.localRotation = Quaternion.Euler(_characters[_currentIndex].spawnRotation);
        }

        UpdateCharacterInfo();
        UpdateConfirmButton();
    }

    private void UpdateCharacterInfo()
    {
        if (_characters.Count == 0 || _currentIndex < 0 || _currentIndex >= _characters.Count) return;

        CharacterData data = _characters[_currentIndex];

        if (_nameText != null) _nameText.text = data.characterName;
        if (_descriptionText != null) _descriptionText.text = data.description;
        if (_attackTypeText != null) _attackTypeText.text = $"Тип атаки: {data.attackType}";
        if (_damageText != null) _damageText.text = $"Урон: {data.damage}";
        if (_healthText != null) _healthText.text = $"Здоровье: {data.health}";
        if (_speedText != null) _speedText.text = $"Скорость: {data.speed}";
    }
}
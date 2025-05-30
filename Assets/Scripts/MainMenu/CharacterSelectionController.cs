using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;



public class CharacterSelectionController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _attackTypeText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _speedText;

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

    private void Start()
    {
        _originalContainerPosition = _modelContainer.position;
        _modelContainer.position = new Vector3(
            _originalContainerPosition.x,
            _showYPosition,
            _originalContainerPosition.z
        );
        ShowCharacter(0);
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

    private IEnumerator SwitchCharacterCoroutine(int newIndex)
    {
        _isAnimating = true;

        // Hide current model
        yield return StartCoroutine(MoveModel(false));

        // Destroy old model
        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        _currentIndex = newIndex;

        // Create new model with custom position/rotation
        if (_characters[_currentIndex].characterPrefab != null)
        {
            _currentModel = Instantiate(
                _characters[_currentIndex].characterPrefab,
                _modelContainer
            );

            // Apply custom position and rotation
            _currentModel.transform.localPosition = _characters[_currentIndex].spawnPositionOffset;
            _currentModel.transform.localRotation = Quaternion.Euler(_characters[_currentIndex].spawnRotation);
        }

        // Show new model
        yield return StartCoroutine(MoveModel(true));

        UpdateCharacterInfo();
        _isAnimating = false;
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

            // Apply custom transform
            _currentModel.transform.localPosition = _characters[_currentIndex].spawnPositionOffset;
            _currentModel.transform.localRotation = Quaternion.Euler(_characters[_currentIndex].spawnRotation);
        }

        UpdateCharacterInfo();
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
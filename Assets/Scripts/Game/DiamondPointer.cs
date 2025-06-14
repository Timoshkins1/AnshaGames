using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiamondPointer : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private RectTransform pointerUI;
    [SerializeField] private Image pointerImage;
    [SerializeField] private float pointerHeight = 0.2f; // ������ ��������� ��� �������

    [Header("Pointer Settings")]
    [SerializeField] private Color farColor = Color.red;
    [SerializeField] private Color closeColor = Color.green;
    [SerializeField] private float updateInterval = 0.3f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float pointerScale = 1f; // ������� ���������

    private Transform nearestDiamond;
    private float lastUpdateTime;
    private bool isInitialized;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => DiamondSpawner.Instance != null);

        if (playerTransform == null)
        {
            Debug.LogError("Player transform reference is missing!");
            yield break;
        }

        if (pointerUI == null || pointerImage == null)
        {
            Debug.LogError("UI references are missing!");
            yield break;
        }

        DiamondSpawner.Instance.OnDiamondSpawned += HandleNewDiamondSpawned;
        isInitialized = true;

        UpdateNearestDiamond();
    }

    private void OnDestroy()
    {
        if (DiamondSpawner.Instance != null)
        {
            DiamondSpawner.Instance.OnDiamondSpawned -= HandleNewDiamondSpawned;
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (Time.time - lastUpdateTime > updateInterval)
        {
            UpdateNearestDiamond();
            lastUpdateTime = Time.time;
        }

        UpdatePointer();
    }

    private void HandleNewDiamondSpawned(Diamond diamond)
    {
        if (diamond == null || diamond.transform == null) return;

        if (nearestDiamond == null ||
            Vector3.Distance(playerTransform.position, diamond.transform.position) <
            Vector3.Distance(playerTransform.position, nearestDiamond.position))
        {
            nearestDiamond = diamond.transform;
        }
    }

    private void UpdateNearestDiamond()
    {
        nearestDiamond = null;
        float closestDistance = Mathf.Infinity;

        if (DiamondSpawner.Instance == null || DiamondSpawner.Instance.DiamondPool == null)
            return;

        foreach (var diamond in DiamondSpawner.Instance.DiamondPool)
        {
            if (diamond == null) continue;
            if (!diamond.gameObject.activeInHierarchy) continue;

            var diamondTransform = diamond.transform;
            if (diamondTransform == null) continue;

            float distance = Vector3.Distance(playerTransform.position, diamondTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestDiamond = diamondTransform;
            }
        }
    }

    private void UpdatePointer()
    {
        if (pointerUI == null || pointerImage == null || playerTransform == null)
        {
            if (pointerUI != null) pointerUI.gameObject.SetActive(false);
            return;
        }

        if (nearestDiamond == null || !nearestDiamond.gameObject.activeInHierarchy)
        {
            pointerUI.gameObject.SetActive(false);
            return;
        }

        pointerUI.gameObject.SetActive(true);

        try
        {
            // ����������� � ������ (������ �� �����������)
            Vector3 dirToDiamond = nearestDiamond.position - playerTransform.position;
            dirToDiamond.y = 0;
            float distance = dirToDiamond.magnitude;

            // ��������� ���� ���������
            pointerImage.color = Color.Lerp(closeColor, farColor, Mathf.Clamp01(distance / maxDistance));

            // ������� ��������� - ��� �������
            Vector3 pointerPosition = playerTransform.position;
            pointerPosition.y += pointerHeight;
            pointerUI.position = pointerPosition;

            // ������������ ���������
            pointerUI.localScale = Vector3.one * pointerScale;

            // ������������ ��������� � ����������� ������ (��� ���� ������)
            if (dirToDiamond != Vector3.zero)
            {
                // ��������� ���� ����� ������������ ������ � ������������ � ������
                float angle = Mathf.Atan2(dirToDiamond.x, dirToDiamond.z) * Mathf.Rad2Deg;

                // ��������� ������� ������ ������ ��� Y
                pointerUI.rotation = Quaternion.Euler(90f, angle, 0f);

                // �������������� ������� (���� ����� ������ ��������� ��������� �������):
                // pointerUI.rotation = Quaternion.Euler(0f, angle, 90f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Pointer update error: {e.Message}");
            pointerUI.gameObject.SetActive(false);
        }
    }
}
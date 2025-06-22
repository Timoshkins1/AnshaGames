using UnityEngine;

public class FallingPrefab : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallDelay = 3f; // �������� ����� ��������
    [SerializeField] private float fallSpeed = 5f; // �������� �������
    [SerializeField] private float destroyDelayAfterFall = 1f; // �������� ����� ������������ ����� ������ �������

    private Rigidbody rb;
    private bool isFalling = false;
    private float fallStartTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ��������� ������ �������
        Invoke(nameof(StartFalling), fallDelay);
    }

    private void StartFalling()
    {
        isFalling = true;
        fallStartTime = Time.time;

        // ���� ���� Rigidbody, ��������� ������������� ���������� (����� ��������� �������)
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (isFalling)
        {
            // ������� ������ ����
            if (rb != null)
            {
                rb.velocity = Vector3.down * fallSpeed;
            }
            else
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            }

            // ���������, �� ���� �� ���������� ������
            if (Time.time - fallStartTime >= destroyDelayAfterFall)
            {
                Destroy(gameObject);
            }
        }
    }
}
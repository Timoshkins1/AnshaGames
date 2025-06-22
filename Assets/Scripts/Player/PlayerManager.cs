using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image playerImageUI;  // �������� � UI (�� Canvas)
    [SerializeField] private TextMeshProUGUI playerNameUI;   // ����� ����� � UI (�� Canvas)

    [Header("Settings")]
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FixedJoystick movementJoystick;
    [SerializeField] private Joystick shootingJoystick;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private AmmoDisplay ammoDisplay;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private PlayerUI playerUI; // ������ �� UI ���������
    [SerializeField] private Slider ultimateSlider;

    [Header("Ultimate Settings")]
    [SerializeField] private Joystick ultimateJoystick; // ����� �������� ��� �����
    public Joystick UltimateJoystick => ultimateJoystick;
    public FixedJoystick MovementJoystick => movementJoystick;
    public Joystick ShootingJoystick => shootingJoystick;
    public Transform PlayerTransform => currentPlayer?.transform;

    private GameObject currentPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �������� ����������� ID ���������
        int selectedCharacterID = PlayerPrefs.GetInt("SelectedCharacterID", 0);

        // ���������, ����� ������ ��� � ���������� ��������
        if (selectedCharacterID < 0 || selectedCharacterID >= playerPrefabs.Length)
        {
            selectedCharacterID = 0;
            Debug.LogWarning("������������ ID ���������. ������������ �������� �� ���������: 0");
        }

        SpawnPlayer(selectedCharacterID);
    }

    public void SpawnPlayer(int playerIndex)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerPrefabs[playerIndex], spawnPoint.position, spawnPoint.rotation);
        InitializePlayerComponents(currentPlayer);

        // ��������� UI ��� ������ ������ ������
        UpdatePlayerUI(currentPlayer);
    }

    private void InitializePlayerComponents(GameObject player)
    {
        var health = player.GetComponent<Health>();
        var controller = player.GetComponent<PlayerController>();
        var shooting = player.GetComponent<PlayerShooting>();
        var animation = player.GetComponent<PlayerAnimation>();

        if (health != null && healthBar != null)
        {
            health.Initialize(healthBar);
        }

        if (controller != null)
        {
            controller.Initialize(movementJoystick);
        }

        if (shooting != null)
        {
            shooting.Initialize(shootingJoystick, ammoDisplay);
        }

        if (animation != null)
        {
            animation.Initialize(movementJoystick);
        }

        if (enemySpawner != null)
        {
            enemySpawner.SetPlayerTransform(PlayerTransform);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetPlayerTransform(player.transform);
        }
        var ultimate = player.GetComponent<PlayerUltimate>();
        if (ultimate != null && ultimateSlider != null)
        {
            ultimate.Initialize(ultimateSlider, ultimateJoystick); // ������ �������� � ��������
        }
    }

    /// <summary>
    /// ��������� UI �� ������ ������ ������.
    /// </summary>
    private void UpdatePlayerUI(GameObject player)
    {
        var playerUI = player.GetComponent<PlayerUI>(); // �������� ������ ������
        if (playerUI != null)
        {
            if (playerImageUI != null)
            {
                playerImageUI.sprite = playerUI.GetPlayerSprite();
            }

            if (playerNameUI != null)
            {
                playerNameUI.text = playerUI.GetPlayerName();
            }
        }
    }
}
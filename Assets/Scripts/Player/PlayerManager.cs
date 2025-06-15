using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image playerImageUI;  // Картинка в UI (на Canvas)
    [SerializeField] private TextMeshProUGUI playerNameUI;   // Текст имени в UI (на Canvas)

    [Header("Settings")]
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FixedJoystick movementJoystick;
    [SerializeField] private Joystick shootingJoystick;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private AmmoDisplay ammoDisplay;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private PlayerUI playerUI; // Ссылка на UI компонент

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
        PlayerManager.Instance.SpawnPlayer(0);
    }

    public void SpawnPlayer(int playerIndex)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerPrefabs[playerIndex], spawnPoint.position, spawnPoint.rotation);
        InitializePlayerComponents(currentPlayer);

        // Обновляем UI при спавне нового игрока
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
       
    }

    /// <summary>
    /// Обновляет UI на основе данных игрока.
    /// </summary>
    private void UpdatePlayerUI(GameObject player)
    {
        var playerUI = player.GetComponent<PlayerUI>(); // Получаем данные игрока
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
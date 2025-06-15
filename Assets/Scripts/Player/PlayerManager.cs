using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FixedJoystick movementJoystick;
    [SerializeField] private Joystick shootingJoystick;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private AmmoDisplay ammoDisplay;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CameraFollow cameraFollow;
    // Добавляем публичные свойства для доступа
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
    }

    private void InitializePlayerComponents(GameObject player)
    {
        // Получаем все необходимые компоненты
        var health = player.GetComponent<Health>();
        var controller = player.GetComponent<PlayerController>();
        var shooting = player.GetComponent<PlayerShooting>();
        var animation = player.GetComponent<PlayerAnimation>();

        // Устанавливаем зависимости через методы, а не напрямую в поля
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

            // Дополнительная проверка после инициализации
            if (animation._joystick == null)
            {
                Debug.LogError("Failed to initialize joystick in PlayerAnimation!", player);
            }
        }
        if (enemySpawner != null)
        {
            enemySpawner.SetPlayerTransform(PlayerTransform);
        }
        // Передаем трансформ игрока камере
        if (cameraFollow != null)
        {
            cameraFollow.SetPlayerTransform(player.transform);
        }
        else
        {
            Debug.LogWarning("CameraFollow reference is not set in PlayerManager!");
        }
    }
}
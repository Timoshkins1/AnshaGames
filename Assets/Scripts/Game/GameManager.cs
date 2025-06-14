using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Настройки игры")]
    [SerializeField] private int initialDiamondsToCollect = 5;
    [SerializeField] private int diamondsIncreasePerWave = 3;
    [SerializeField] private float timeBetweenWaves = 15f;

    [Header("UI")]
    [SerializeField] private TMP_Text diamondsText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private GameObject gameOverPanel;

    private int currentWave = 0;
    private int diamondsCollected = 0;
    private int diamondsToCollect = 0;
    private bool gameOver = false;

    public static GameManager Instance { get; private set; }

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
        StartNewWave();
    }

    public void DiamondCollected()
    {
        if (gameOver) return;

        diamondsCollected++;
        UpdateUI();

        if (diamondsCollected >= diamondsToCollect)
        {
            Invoke(nameof(StartNewWave), timeBetweenWaves);
        }
    }

    private void StartNewWave()
    {
        currentWave++;
        diamondsCollected = 0;
        diamondsToCollect = initialDiamondsToCollect + (currentWave - 1) * diamondsIncreasePerWave;

        UpdateUI();
        DiamondSpawner.Instance.SpawnWave(diamondsToCollect);
        EnemySpawner.Instance.StartWave(currentWave);
    }

    private void UpdateUI()
    {
        diamondsText.text = $"Алмазы: {diamondsCollected}/{diamondsToCollect}";
        waveText.text = $"Волна: {currentWave}";
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
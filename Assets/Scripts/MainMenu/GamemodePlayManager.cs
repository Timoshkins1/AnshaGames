using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamemodePlayManager : MonoBehaviour
{
    [SerializeField] private string gamemodeName;
    [SerializeField] private string gamemodeSceneName;
    [SerializeField] private Sprite Icon;
    [SerializeField] private TextMeshProUGUI gamemodeNameText;
    [SerializeField] private Image IconImage;

    void Start()
    {
        gamemodeNameText.text = gamemodeName;
        IconImage.sprite = Icon;
    }

    public void LoadGamemode()
    {
        if (!string.IsNullOrEmpty(gamemodeSceneName))
        {
            // ������� ��������� � �������� ��� ��� �����
            SceneLoader loader = FindObjectOfType<SceneLoader>();
            if (loader != null)
            {
                loader.LoadTargetScene(gamemodeSceneName);
            }
            else
            {
                Debug.LogError("SceneLoader �� ������ �� �����!");
                SceneManager.LoadScene(gamemodeSceneName); // ������
            }
        }
        else
        {
            Debug.LogError("��� ����� �� �������!");
        }
    }
}
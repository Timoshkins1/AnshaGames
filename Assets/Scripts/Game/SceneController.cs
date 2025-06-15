using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // �������� ����� ��� �������� (������� � ����������)
    public string targetSceneName;

    // ����� ��� ������������ ������� �����
    public void RestartCurrentScene()
    {
        // �������� ��� ������� �����
        string currentSceneName = SceneManager.GetActiveScene().name;
        // ��������� � ������
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneName);
    }

    // ����� ��� �������� ������ �����
    public void LoadTargetScene()
    {
        // ���������, ������� �� ��� �����
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("Target scene name is not set in SceneController!");
        }
    }
}
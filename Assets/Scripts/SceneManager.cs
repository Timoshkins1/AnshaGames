using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // �������� ��������� ����� (����� ������ � ����������)
    public string nextSceneName;

    // ����� ��� �������� ��������� �����
    public void LoadNextScene()
    {
        // ���������, ������ �� ��� �����
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // ��������� ����� �� �����
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set!");
        }
    }
}
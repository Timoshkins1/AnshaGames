using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Название сцены для загрузки (укажите в инспекторе)
    public string targetSceneName;

    // Метод для перезагрузки текущей сцены
    public void RestartCurrentScene()
    {
        // Получаем имя текущей сцены
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Загружаем её заново
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneName);
    }

    // Метод для загрузки другой сцены
    public void LoadTargetScene()
    {
        // Проверяем, указано ли имя сцены
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
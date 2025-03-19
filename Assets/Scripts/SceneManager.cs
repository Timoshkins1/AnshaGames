using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Название следующей сцены (можно задать в инспекторе)
    public string nextSceneName;

    // Метод для загрузки следующей сцены
    public void LoadNextScene()
    {
        // Проверяем, задано ли имя сцены
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Загружаем сцену по имени
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set!");
        }
    }
}
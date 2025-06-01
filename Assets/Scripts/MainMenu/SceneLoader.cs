using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _progressBar;

    private AsyncOperation _loadingOperation;
    private string _targetSceneName;

    // Публичный метод для установки сцены
    public void LoadTargetScene(string sceneName)
    {
        _targetSceneName = sceneName;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        if (string.IsNullOrEmpty(_targetSceneName))
        {
            Debug.LogError("Не указано имя сцены для загрузки");
            yield break;
        }

        _loadingScreen.SetActive(true);

        _loadingOperation = SceneManager.LoadSceneAsync(_targetSceneName);
        _loadingOperation.allowSceneActivation = false;

        while (!_loadingOperation.isDone)
        {
            float progress = Mathf.Clamp01(_loadingOperation.progress / 0.9f);
            _progressBar.value = progress;

            if (_loadingOperation.progress >= 0.9f)
            {
                // Дополнительное ожидание для плавности
                yield return new WaitForSeconds(0.5f);
                _loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
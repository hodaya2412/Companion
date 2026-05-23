using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderButton : MonoBehaviour
{
    public string sceneName;

    public void Load()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        // Fade Out
        if (SceneFader.Instance != null)
            yield return SceneFader.Instance.FadeOutAndLoad(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}

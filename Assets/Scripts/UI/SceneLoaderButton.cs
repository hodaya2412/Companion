using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderButton : MonoBehaviour
{
    public string sceneName;
    [SerializeField] private float normalTimeScale = 1f;
    public void Load()
    {
        Time.timeScale = normalTimeScale;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        
        if (SceneFader.Instance != null)
            yield return SceneFader.Instance.FadeOutAndLoad(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}

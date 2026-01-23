using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderButton : MonoBehaviour
{
    public string sceneName;

    public void Load()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        // Fade Out
        if (SceneFader.Instance != null)
            yield return SceneFader.Instance.FadeOutAndLoad(sceneName);
    }
}

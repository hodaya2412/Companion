using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeSpeed = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage != null)
        {
            canvasGroup = fadeImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);

        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
    }

    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }
}
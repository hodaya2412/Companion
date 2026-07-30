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

    [SerializeField] private float opaqueAlpha = 1f;
    [SerializeField] private float transparentAlpha = 0f;

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

            canvasGroup.alpha = opaqueAlpha;
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

        while (canvasGroup.alpha > transparentAlpha)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = transparentAlpha;
        canvasGroup.blocksRaycasts = false;

        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
    }

    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha < opaqueAlpha)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = opaqueAlpha;

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

    public IEnumerator FadeOut()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha < opaqueAlpha)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = opaqueAlpha;
    }
}
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
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // CanvasGroup לניהול alpha ו־input blocking
        if (fadeImage != null)
        {
            canvasGroup = fadeImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true; // חוסם input בהתחלה
        }

        // מאזין לטעינת סצנה חדשה
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Fade In בתחילת המשחק
        StartCoroutine(FadeIn());
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameStateManager.Instance.SetState(GameState.Playing);
        // Fade In אחרי טעינת כל סצנה חדשה
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true; // חוסם input בזמן fade

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false; // מאפשר input
        GameEvents.OnStateChanged?.Invoke(GameState.Playing); // מוודא מצב נכון אחרי FadeIn
    }

    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = true; // חוסם input בזמן fade

        // Fade Out
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // טוענים אסינכרונית את הסצנה החדשה
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade In יקרה אוטומטית ב־OnSceneLoaded
    }

    // פונקציה נוחה לקריאה מסקריפט אחר
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }
}

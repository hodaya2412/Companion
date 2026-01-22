using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDPersist : MonoBehaviour
{
    private static HUDPersist instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var bar = GetComponentInChildren<HealthBarUI>(true);
        if (bar == null) return;

        var playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            bar.playerHealth = playerHealth;
        }
    }
}

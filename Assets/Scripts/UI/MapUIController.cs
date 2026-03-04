using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIController : MonoBehaviour
{
    public GameObject mapPanel;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CloseMap();
    }

    public void ToggleMap()
    {
        if (mapPanel == null) return;

        bool isActive = mapPanel.activeSelf;
        mapPanel.SetActive(!isActive);

        if (!isActive)
            GameStateManager.Instance.SetState(GameState.Map);
        else
            GameStateManager.Instance.SetState(GameState.Playing);
    }

    public void CloseMap()
    {
        if (mapPanel == null) return;

        mapPanel.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Playing);
    }
}
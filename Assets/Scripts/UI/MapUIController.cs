using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIController : MonoBehaviour
{
    public GameObject mapPanel;

    private GameplayState currentGameplayState;
    private UIState currentUIState;

    private void Awake()
    {
        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        currentUIState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged += HandleUIStateChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged -= HandleUIStateChanged;
    }

    private void HandleGameplayStateChanged(GameplayState newState)
    {
        currentGameplayState = newState;

        if (newState == GameplayState.Combat && mapPanel != null && mapPanel.activeSelf)
            CloseMapSilently();
    }

    private void HandleUIStateChanged(UIState newState)
    {
        currentUIState = newState;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CloseMapSilently();
    }

    public void ToggleMap()
    {
        if (mapPanel == null) return;

        bool gameplayAllowsMap = currentGameplayState == GameplayState.Playing;
        bool uiAllowsMap = currentUIState == UIState.None || currentUIState == UIState.Map;

        if (!gameplayAllowsMap || !uiAllowsMap)
            return;

        bool shouldBeActive = !mapPanel.activeSelf;
        mapPanel.SetActive(shouldBeActive);
        if (shouldBeActive)
        {
            GetComponentInChildren<MapAttentionSeeker>()?.StopGlow();
        }
        GameEvents.RequestUIStateChange?.Invoke(shouldBeActive ? UIState.Map : UIState.None);
    }

    public void CloseMap()
    {
        if (mapPanel == null) return;

        mapPanel.SetActive(false);
        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
    }

    private void CloseMapSilently()
    {
        if (mapPanel == null) return;
        mapPanel.SetActive(false);
    }
}
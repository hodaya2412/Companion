using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpUIController : MonoBehaviour
{
    public GameObject helpPanel;

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

        if (newState != GameplayState.Playing && helpPanel != null && helpPanel.activeSelf)
        {
            CloseHelpSilently();

            if (currentUIState == UIState.Help)
                GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        }
    }

    private void HandleUIStateChanged(UIState newState)
    {
        currentUIState = newState;

       
        if (newState != UIState.Help && newState != UIState.None && helpPanel != null && helpPanel.activeSelf)
        {
            CloseHelpSilently();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CloseHelpSilently();

        if (currentUIState == UIState.Help)
            GameEvents.RequestUIStateChange?.Invoke(UIState.None);
    }

    public void ToggleHelp()
    {
        if (helpPanel == null) return;

        bool isOpening = !helpPanel.activeSelf;

        if (isOpening)
        {
            bool gameplayAllowsHelp = currentGameplayState == GameplayState.Playing;
            bool uiAllowsHelp = currentUIState == UIState.None || currentUIState == UIState.Help;

            if (!gameplayAllowsHelp || !uiAllowsHelp)
                return;

            helpPanel.SetActive(true);
            GameEvents.RequestUIStateChange?.Invoke(UIState.Help);
        }
        else
        {
            helpPanel.SetActive(false);

            if (currentUIState == UIState.Help)
                GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        }
    }

    public void CloseHelp()
    {
        if (helpPanel == null) return;

        helpPanel.SetActive(false);

        if (currentUIState == UIState.Help)
            GameEvents.RequestUIStateChange?.Invoke(UIState.None);
    }

    private void CloseHelpSilently()
    {
        if (helpPanel == null) return;
        helpPanel.SetActive(false);
    }
}
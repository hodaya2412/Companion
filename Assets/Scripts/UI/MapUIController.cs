using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUIController : MonoBehaviour
{
    public GameObject mapPanel;
    private GameState currentGameState;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // האזנה לעדכוני מצב משחק
        GameEvents.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEvents.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState) => currentGameState = newState;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CloseMap();
    }

    public void ToggleMap()
    {
        if (mapPanel == null) return;

        // הגנה: אל תפתח מפה אם אנחנו בדיאלוג או במצב אחר שלא מאפשר זאת
        if (currentGameState != GameState.Playing && currentGameState != GameState.Map) return;

        bool shouldBeActive = !mapPanel.activeSelf;
        mapPanel.SetActive(shouldBeActive);

        // שימוש ב-Events במקום ב-Instance
        GameState nextState = shouldBeActive ? GameState.Map : GameState.Playing;
        GameEvents.RequestStateChange?.Invoke(nextState);
    }

    public void CloseMap()
    {
        if (mapPanel == null) return;

        mapPanel.SetActive(false);
        GameEvents.RequestStateChange?.Invoke(GameState.Playing);
    }
}
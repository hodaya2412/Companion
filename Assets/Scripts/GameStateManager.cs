using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameplayState
{
    Playing,
    Combat,
    BeingGuided
}

public enum UIState
{
    None,
    Dialogue,
    Inventory,
    Map,
    Choice
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameplayState CurrentGameplayState { get; private set; } = GameplayState.Playing;
    public UIState CurrentUIState { get; private set; } = UIState.None;

    private readonly HashSet<string> gameFlags = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        GameEvents.RequestCurrentGameplayState += GetCurrentGameplayState;
        GameEvents.RequestCurrentUIState += GetCurrentUIState;

        GameEvents.RequestGameplayStateChange += SetGameplayState;
        GameEvents.RequestUIStateChange += SetUIState;

        GameEvents.RequestFlagState += GetFlag;
    }

    private void OnDisable()
    {
        GameEvents.RequestCurrentGameplayState -= GetCurrentGameplayState;
        GameEvents.RequestCurrentUIState -= GetCurrentUIState;

        GameEvents.RequestGameplayStateChange -= SetGameplayState;
        GameEvents.RequestUIStateChange -= SetUIState;

        GameEvents.RequestFlagState -= GetFlag;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetUIState(UIState.None);
        SetGameplayState(GameplayState.Playing);
    }

    public void SetGameplayState(GameplayState newState)
    {
        if (CurrentGameplayState == newState) return;

        CurrentGameplayState = newState;
        GameEvents.OnGameplayStateChanged?.Invoke(CurrentGameplayState);

        Debug.Log($"[GameStateManager] GameplayState changed to: {CurrentGameplayState}");
    }

    public void SetUIState(UIState newState)
    {
        if (CurrentUIState == newState) return;

        CurrentUIState = newState;
        GameEvents.OnUIStateChanged?.Invoke(CurrentUIState);

        Debug.Log($"[GameStateManager] UIState changed to: {CurrentUIState}");
    }

    public bool GetFlag(string key)
    {
        return gameFlags.Contains(key);
    }

    public void SetFlag(string key, bool value = true)
    {
        if (value) gameFlags.Add(key);
        else gameFlags.Remove(key);

        GameEvents.OnFlagChanged?.Invoke(key, value);
        Debug.Log($"[GameStateManager] Flag '{key}' set to {value}");
    }

    public void ResetFlag(string key)
    {
        if (gameFlags.Remove(key))
            GameEvents.OnFlagChanged?.Invoke(key, false);
    }

    public bool CheckFlag(string key, bool expectedValue = true)
    {
        return GetFlag(key) == expectedValue;
    }

    private GameplayState GetCurrentGameplayState() => CurrentGameplayState;
    private UIState GetCurrentUIState() => CurrentUIState;
}
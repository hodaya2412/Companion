using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Dialogue,
    Inventory,
    Map,
    BeingGuided,
    Choice
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Playing;

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
        SetState(GameState.Playing);
    }

    
    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        GameEvents.OnStateChanged?.Invoke(CurrentState);
        Debug.Log("Game state changed to: " + CurrentState);
    }

    
    public bool GetFlag(string key)
    {
        return gameFlags.Contains(key);
    }

    
    public void SetFlag(string key, bool value = true)
    {
        if (value)
            gameFlags.Add(key);
        else
            gameFlags.Remove(key);

        GameEvents.OnFlagChanged?.Invoke(key, value);
        Debug.Log($"Flag '{key}' set to {value}");
    }

    public void ResetFlag(string key)
    {
        if (gameFlags.Remove(key))
        {
            GameEvents.OnFlagChanged?.Invoke(key, false);
        }
    }

    public bool CheckFlag(string key, bool expectedValue = true)
    {
        return GetFlag(key) == expectedValue;
    }

    private void OnEnable()
    {
        GameEvents.RequestCurrentGameState += GetCurrentState;
        GameEvents.RequestFlagState += GetFlag;
    }

    private void OnDisable()
    {
        GameEvents.RequestCurrentGameState -= GetCurrentState;
        GameEvents.RequestFlagState -= GetFlag;

    }

    private GameState GetCurrentState()
    {
        return CurrentState;
    }
}
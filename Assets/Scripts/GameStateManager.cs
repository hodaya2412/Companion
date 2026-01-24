using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,    
    Dialogue,  
    Inventory,  
    Map,        
    BeingGuided
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // מצב ברירת מחדל אחרי מעבר סצנה
        SetState(GameState.Playing);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        GameEvents.OnStateChanged?.Invoke(CurrentState);
        Debug.Log("Game state changed to: " + CurrentState);
    }
}
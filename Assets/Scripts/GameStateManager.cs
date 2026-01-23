using System;
using UnityEngine;

public enum GameState
{
    Playing,    // השחקן יכול לזוז וללחוץ על כפתורים
    Dialogue,   // השחקן לא יכול לזוז, כפתורים נעולים
    Inventory,  // השחקן צופה באינוונטורי
    Map,        // השחקן צופה במפה
    BeingGuided
}



public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    // אירוע שמודיע לכל המערכות שהמצב השתנה

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        GameEvents.OnStateChanged?.Invoke(CurrentState);
        Debug.Log("Game state changed to: " + CurrentState);
    }
}


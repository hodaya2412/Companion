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

    // 🟢 סטטוס אוניברסלי – כל דגל הוא מחרוזת
    private Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();

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
        SetState(GameState.Playing);
    }

    // ✅ ניהול מצב המשחק הכללי
    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        GameEvents.OnStateChanged?.Invoke(CurrentState);
        Debug.Log("Game state changed to: " + CurrentState);
    }

    // 🟢 פונקציות ניהול סטטוסים אוניברסליים (Flags)
    public bool GetFlag(string key)
    {
        if (gameFlags.ContainsKey(key)) return gameFlags[key];
        return false; // ברירת מחדל = false
    }

    public void SetFlag(string key, bool value = true)
    {
        gameFlags[key] = value;
        Debug.Log($"Flag '{key}' set to {value}");
    }

    public void ResetFlag(string key)
    {
        if (gameFlags.ContainsKey(key))
            gameFlags[key] = false;
    }

    // 🟢 פונקציה לבדיקת מצב Flag - שימוש בדיאלוגים מותנים
    public bool CheckFlag(string key, bool expectedValue = true)
    {
        return GetFlag(key) == expectedValue;
    }
}
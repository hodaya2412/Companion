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
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // מונע מהמנהל להימחק במעבר בין סצנות אם תרצה לשמור על Flags
        // DontDestroyOnLoad(gameObject); 

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        // רישום לאירועים ובקשות
        GameEvents.RequestCurrentGameState += GetCurrentState;
        GameEvents.RequestFlagState += GetFlag;

        // הוספנו: האזנה לבקשת שינוי מצב מאירועים חיצוניים
        GameEvents.RequestStateChange += SetState;
    }

    private void OnDisable()
    {
        // ניקוי רישומים למניעת דליפות זיכרון
        GameEvents.RequestCurrentGameState -= GetCurrentState;
        GameEvents.RequestFlagState -= GetFlag;
        GameEvents.RequestStateChange -= SetState;
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
        // איפוס מצב בכל טעינת סצנה חדשה
        SetState(GameState.Playing);
    }

    /// <summary>
    /// משנה את מצב המשחק ומפיץ אירוע לכל מי שמקשיב
    /// </summary>
    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        // שליחת עדכון לכל מי שמאזין (כמו InventoryUI או PlayerController)
        GameEvents.OnStateChanged?.Invoke(CurrentState);

        Debug.Log($"[GameStateManager] State changed to: {CurrentState}");
    }

    #region Flag Management

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
        Debug.Log($"[GameStateManager] Flag '{key}' set to {value}");
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

    #endregion

    private GameState GetCurrentState()
    {
        return CurrentState;
    }
}
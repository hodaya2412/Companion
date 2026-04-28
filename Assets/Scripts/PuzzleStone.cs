using UnityEngine;
using UnityEngine.InputSystem; // חובה להוסיף

public class PuzzleStone : MonoBehaviour
{
    public string puzzleID = "ForestStones";
    public string solvedFlag = "Forest_PuzzleSolved";

    [Header("Block After Victory")]
    [Tooltip("שם הדגל שסימנו ב-GameStateManager כשהשודדים מתו")]
    public string banditsDefeatedFlag = "Forest_BanditsDefeated";

    private InputActions inputActions;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        // הפעלת ה-Input ורישום לפעולת ה-Interact מהמפה שיצרת
        inputActions.Interact.Enable();
        inputActions.Interact.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        // ביטול רישום וכיבוי
        inputActions.Interact.Interact.performed -= OnInteractPerformed;
        inputActions.Interact.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        // בדיקה שהשחקן פיזית ליד האבן
        if (!isPlayerInRange) return;

        TryOpenStonePuzzle();
    }

    private void TryOpenStonePuzzle()
    {
        if (GameStateManager.Instance == null) return;

        // 1. בדיקה: אם החידה כבר נפתרה - אל תעשה כלום
        if (GameStateManager.Instance.GetFlag(solvedFlag))
            return;

        // 2. בדיקה: האם השודדים כבר חוסלו?
        if (GameStateManager.Instance.GetFlag(banditsDefeatedFlag))
        {
            Debug.Log("השודדים מתו, האבן כבר לא מגיבה.");
            return;
        }

        // 3. בדיקת מצבי המשחק הרגילים
        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canOpenPuzzle =
            (gameplayState == GameplayState.Playing || gameplayState == GameplayState.Combat) &&
            uiState == UIState.None;

        if (canOpenPuzzle)
        {
            Debug.Log($"Opening puzzle: {puzzleID} via E key");
            GameEvents.OnPuzzleStoneClicked?.Invoke(puzzleID);
        }
    }

    // --- זיהוי טווח (Trigger) ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
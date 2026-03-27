using UnityEngine;

public class PuzzleStone : MonoBehaviour
{
    public string puzzleID = "ForestStones";
    public string solvedFlag = "Forest_PuzzleSolved";

    [Header("Block After Victory")]
    [Tooltip("שם הדגל שסימנו ב-GameStateManager כשהשודדים מתו")]
    public string banditsDefeatedFlag = "Forest_BanditsDefeated";

    private void OnMouseDown()
    {
        if (GameStateManager.Instance == null) return;

        // 1. בדיקה: אם החידה כבר נפתרה - אל תעשה כלום
        if (GameStateManager.Instance.GetFlag(solvedFlag))
            return;

        // 2. בדיקה: האם השודדים כבר חוסלו? אם כן - האבן חסומה
        if (GameStateManager.Instance.GetFlag(banditsDefeatedFlag))
        {
            Debug.Log("השודדים מתו, האבן כבר לא מגיבה.");
            return; // עוצר כאן ולא פותח את החידה
        }

        // 3. בדיקת מצבי המשחק הרגילים (UI פתוח וכו')
        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canOpenPuzzle =
            (gameplayState == GameplayState.Playing || gameplayState == GameplayState.Combat) &&
            uiState == UIState.None;

        if (canOpenPuzzle)
        {
            GameEvents.OnPuzzleStoneClicked?.Invoke(puzzleID);
        }
    }
}
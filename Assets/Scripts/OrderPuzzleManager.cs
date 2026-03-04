using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class OrderPuzzleManager : MonoBehaviour
{
    [Header("Identity & State")]
    [Tooltip("חייב להיות זהה ל-ID שעל האבנים בעולם")]
    public string puzzleID = "ForestStones";
    public string solvedFlag = "Forest_PuzzleSolved";

    [Header("UI References")]
    public GameObject puzzlePanel;
    public Button checkButton;
    public Button closeButton;

    [Header("Puzzle Logic")]
    [Tooltip("גררי לכאן את האבנים מה-UI בסדר הנכון מלמטה למעלה (גדולה, בינונית, קטנה)")]
    public List<RectTransform> puzzleStonesInOrder;

    [Header("Actions to Execute on Win")]
    public List<DialogueAction> onWinActions;

    private void OnEnable()
    {
        // האזנה לאירוע פתיחת החידה מהעולם
        GameEvents.OnPuzzleStoneClicked += HandlePuzzleTrigger;

        if (checkButton != null) checkButton.onClick.AddListener(CheckSolution);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePuzzle);
    }

    private void OnDisable()
    {
        // הסרת רישום למניעת בעיות זיכרון
        GameEvents.OnPuzzleStoneClicked -= HandlePuzzleTrigger;

        if (checkButton != null) checkButton.onClick.RemoveListener(CheckSolution);
        if (closeButton != null) closeButton.onClick.RemoveListener(ClosePuzzle);
    }

    private void Start()
    {
        // לוודא שהפאנל סגור בהתחלה
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
    }

    private void HandlePuzzleTrigger(string triggeredID)
    {
        // אם מישהו בעולם צעק את ה-ID שלי - אני נפתח
        if (triggeredID == puzzleID)
        {
            // בדיקה אם כבר נפתר בעבר (ליתר ביטחון)
            if (GameStateManager.Instance.GetFlag(solvedFlag)) return;

            OpenPuzzle();
        }
    }

    public void OpenPuzzle()
    {
        puzzlePanel.SetActive(true);
        // משנה מצב כדי שהשחקן לא יזוז בזמן החידה
        GameStateManager.Instance.SetState(GameState.Choice);
    }

    public void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    private void CheckSolution()
    {
        if (puzzleStonesInOrder == null || puzzleStonesInOrder.Count < 2) return;

        bool isCorrect = true;

        // עוברים על הרשימה ובודקים שכל אבן גבוהה יותר (בציר Y) מהקודמת לה
        for (int i = 0; i < puzzleStonesInOrder.Count - 1; i++)
        {
            float currentY = puzzleStonesInOrder[i].anchoredPosition.y;
            float nextY = puzzleStonesInOrder[i + 1].anchoredPosition.y;

            if (nextY <= currentY)
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("Puzzle Solved Successfully!");
            ExecuteWin();
        }
        else
        {
            Debug.Log("Incorrect Order. Try again!");
            // כאן אפשר להוסיף אנימציה של רעידה או צליל שגיאה
        }
    }

    private void ExecuteWin()
    {
        // הפעלת כל ה-Actions (כולל SetFlagAction שיצרת ב-Unity)
        foreach (var action in onWinActions)
        {
            if (action != null) action.Execute();
        }

        ClosePuzzle();
    }
}
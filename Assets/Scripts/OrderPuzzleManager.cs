using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class OrderPuzzleManager : MonoBehaviour
{
    [Header("Identity & State")]
    public string puzzleID = "ForestStones";
    public string solvedFlag = "Forest_PuzzleSolved";

    [Header("UI References")]
    public GameObject puzzlePanel;
    public Button checkButton;
    public Button closeButton;
    public TMP_Text timerText; // הטיימר בתוך החידה
    public TMP_Text worldCooldownText; // טקסט חדש: להראות לשחקן כמה זמן לחכות (אופציונלי)

    [Header("Puzzle Timing")]
    public float puzzleDuration = 60f;
    public float baseCooldown = 120f; // 2 דקות בסיס
    public float penaltyPerFailure = 30f; // תוספת של 30 שניות על כל טעות

    private float currentTimer;
    private bool isTimerRunning;

    // משתנים סטטיים כדי שישמרו בין פתיחות של הפאנל
    private static float nextAllowedAttemptTime = 0f;
    private static float currentPenaltyTotal = 0f;

    [Header("Puzzle Logic")]
    public List<RectTransform> puzzleStonesInOrder;
    public List<DialogueAction> onWinActions;

    private void OnEnable()
    {
        GameEvents.OnPuzzleStoneClicked += HandlePuzzleTrigger;
        if (checkButton != null) checkButton.onClick.AddListener(CheckSolution);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePuzzle);
    }

    private void OnDisable()
    {
        GameEvents.OnPuzzleStoneClicked -= HandlePuzzleTrigger;
        if (checkButton != null) checkButton.onClick.RemoveListener(CheckSolution);
        if (closeButton != null) closeButton.onClick.RemoveListener(ClosePuzzle);
    }

    private void Update()
    {
        // 1. ניהול הטיימר בתוך החידה
        if (isTimerRunning)
        {
            currentTimer -= Time.deltaTime;
            if (timerText != null) timerText.text = "Time Left: " + Mathf.Ceil(currentTimer).ToString();
            if (currentTimer <= 0) FailPuzzle();
        }

        // 2. עדכון זמן ההמתנה בעולם (אם השחקן ב-Cooldown)
        if (Time.time < nextAllowedAttemptTime && worldCooldownText != null)
        {
            float waitTime = nextAllowedAttemptTime - Time.time;
            worldCooldownText.gameObject.SetActive(true);
            worldCooldownText.text = "Wait " + Mathf.Ceil(waitTime) + "s to try again";
        }
        else if (worldCooldownText != null)
        {
            worldCooldownText.gameObject.SetActive(false);
        }
    }

    private void HandlePuzzleTrigger(string triggeredID)
    {
        if (triggeredID == puzzleID)
        {
            if (GameStateManager.Instance.GetFlag(solvedFlag)) return;

            if (Time.time < nextAllowedAttemptTime)
            {
                Debug.Log("Still in cooldown!");
                return;
            }

            OpenPuzzle();
        }
    }

    public void OpenPuzzle()
    {
        puzzlePanel.SetActive(true);
        GameStateManager.Instance.SetState(GameState.Choice);
        currentTimer = puzzleDuration;
        isTimerRunning = true;

        GameEvents.OnPuzzleOpened?.Invoke(puzzleID);
    }

    public void ClosePuzzle()
    {
        isTimerRunning = false;
        puzzlePanel.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    private void CheckSolution()
    {
        bool isCorrect = true;
        for (int i = 0; i < puzzleStonesInOrder.Count - 1; i++)
        {
            if (puzzleStonesInOrder[i + 1].anchoredPosition.y <= puzzleStonesInOrder[i].anchoredPosition.y)
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            isTimerRunning = false;
            currentPenaltyTotal = 0; // איפוס הקנסות בהצלחה
            ExecuteWin();
        }
        else
        {
            FailPuzzle();
        }
    }

    private void FailPuzzle()
    {
        isTimerRunning = false;

        // חישוב הקנס: זמן בסיס + (מספר טעויות * 30 שניות)
        float totalWait = baseCooldown + currentPenaltyTotal;
        nextAllowedAttemptTime = Time.time + totalWait;

        // הוספת 30 שניות לקנס של הפעם הבאה
        currentPenaltyTotal += penaltyPerFailure;

        ClosePuzzle();
        GameEvents.OnCombatTriggered?.Invoke();
    }

    private void ExecuteWin()
    {
        foreach (var action in onWinActions)
            if (action != null) action.Execute();
        ClosePuzzle();
    }
}
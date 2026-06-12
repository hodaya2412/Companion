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
    public TMP_Text timerText;
    public TMP_Text worldCooldownText;

    [Header("Puzzle Timing")]
    public float puzzleDuration = 60f;
    public float baseCooldown = 120f;
    public float penaltyPerFailure = 30f;

    private float currentTimer;
    private bool isTimerRunning;

    private float nextAllowedAttemptTime = 0f;
    private float currentPenaltyTotal = 0f;

    [Header("Puzzle Logic")]
    public List<RectTransform> puzzleStonesInOrder;
    public List<DialogueAction> onWinActions;

    private void OnEnable()
    {
        GameEvents.OnPuzzleStoneClicked += HandlePuzzleTrigger;

        if (checkButton != null)
            checkButton.onClick.AddListener(CheckSolution);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePuzzle);
    }

    private void OnDisable()
    {
        GameEvents.OnPuzzleStoneClicked -= HandlePuzzleTrigger;

        if (checkButton != null) checkButton.onClick.RemoveListener(CheckSolution);
        if (closeButton != null) closeButton.onClick.RemoveListener(ClosePuzzle);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTimer -= Time.deltaTime;

            if (timerText != null)
                timerText.text = "Time Left: " + Mathf.Ceil(currentTimer);

            if (currentTimer <= 0)
                FailPuzzle();
        }

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
        if (triggeredID != puzzleID) return;
        if (GameStateManager.Instance == null) return;
        if (GameStateManager.Instance.GetFlag(solvedFlag)) return;

        if (GameStateManager.Instance.CurrentGameplayState == GameplayState.Combat)
        {
            Debug.Log("[OrderPuzzleManager] Cannot open puzzle during combat.");
            return;
        }

        if (Time.time < nextAllowedAttemptTime)
        {
            Debug.Log("[OrderPuzzleManager] Still in cooldown!");
            return;
        }

        OpenPuzzle();
    }

    public void OpenPuzzle()
    {
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.CurrentGameplayState == GameplayState.Combat)
        {
            Debug.Log("[OrderPuzzleManager] Puzzle open blocked because combat is active.");
            return;
        }

        if (puzzlePanel == null) return;

        puzzlePanel.SetActive(true);
        GameEvents.RequestUIStateChange?.Invoke(UIState.Choice);

        currentTimer = puzzleDuration;
        isTimerRunning = true;
    }

    public void ClosePuzzle()
    {
        isTimerRunning = false;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
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
            currentPenaltyTotal = 0f;
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

        float totalWait = baseCooldown + currentPenaltyTotal;
        nextAllowedAttemptTime = Time.time + totalWait;
        currentPenaltyTotal += penaltyPerFailure;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Combat);
        GameEvents.OnCombatTriggered?.Invoke();
    }

    private void ExecuteWin()
    {
        currentPenaltyTotal = 0f;

        foreach (var action in onWinActions)
        {
            if (action != null)
                action.Execute();
        }
        GameEvents.OnPuzzleOrCombatSolved?.Invoke();
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
        ClosePuzzle();
    }
}
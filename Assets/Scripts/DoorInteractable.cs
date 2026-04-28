using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; // חובה

public class DoorInteractable : MonoBehaviour, IPuzzlePanelOwner
{
    [Header("References")]
    public PuzzlePanelController puzzlePanelController;
    public ItemGiver itemGiver;

    [Header("Events")]
    public UnityEvent onPuzzleOpened;
    public UnityEvent onPuzzleClosed;

    private InputActions inputActions; // אותו קובץ שקיים אצלך בקרב
    private bool isOpen = false;
    private bool canOpenPuzzle = false;
    private bool isPlayerInRange = false;

    private float lastInteractTime;
    private float interactCooldown = 0.2f;

    private void Awake()
    {
        // יצירת מופע של ה-Input Actions
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        // הפעלת ה-Input ורישום לפעולה (נניח שקראת לה Interact בתוך מפת ה-Player)
        inputActions.Interact.Enable();
        inputActions.Interact.Interact.performed += OnInteractPerformed;

        GameEvents.OnDialogueEvent += HandleDialogueEvent;
    }

    private void OnDisable()
    {
        // ביטול רישום וכיבוי
        inputActions.Interact.Interact.performed -= OnInteractPerformed;
        inputActions.Interact.Disable();

        GameEvents.OnDialogueEvent -= HandleDialogueEvent;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!isPlayerInRange) return;

        // מניעת לחיצות כפולות בטעות
        if (Time.time < lastInteractTime + interactCooldown) return;
        lastInteractTime = Time.time;

        if (isOpen)
        {
            Debug.Log("Closing puzzle with E");
            ClosePuzzle();
        }
        else
        {
            Debug.Log("Trying to open puzzle with E");
            TryOpenPuzzle();
        }
    }

    private void TryOpenPuzzle()
    {
        // בדיקת מצבי משחק (כמו שעשית ב-Combat)
        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canInteract = gameplayState == GameplayState.Playing && uiState == UIState.None;

        if (!canInteract || !canOpenPuzzle || isOpen) return;

        // בדיקת ItemGiver
        if (itemGiver != null && itemGiver.giveOnlyOnce && itemGiver.AlreadyGiven)
            return;

        OpenPuzzle();
    }

    private void OpenPuzzle()
    {
        isOpen = true;

        if (puzzlePanelController != null)
            puzzlePanelController.Open(this);

        onPuzzleOpened?.Invoke();
    }

    // זיהוי כניסת שחקן לטווח הדלת
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player in range of door. Press E to interact.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // --- לוגיקה קיימת ---

    private void HandleDialogueEvent(DialogueAction action)
    {
        if (action is TriggerGameEventAction triggerAction)
        {
            if (triggerAction.eventType == DialogueGameEventType.UnlockPuzzle)
            {
                canOpenPuzzle = true;
            }
        }
    }

    public void ClosePuzzle()
    {
        if (puzzlePanelController != null)
            puzzlePanelController.RequestClose();
    }

    public void OnPuzzlePanelClosed()
    {
        isOpen = false;
        onPuzzleClosed?.Invoke();
    }
}
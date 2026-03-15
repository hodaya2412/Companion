using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : MonoBehaviour
{
    public GameObject puzzlePanel;
    public UnityEvent onPuzzleOpened;
    public UnityEvent onPuzzleClosed;

    private bool isOpen = false;
    private bool canOpenPuzzle = false;

    private void OnEnable()
    {
        GameEvents.OnDialogueEvent += HandleDialogueEvent;
    }

    private void OnDisable()
    {
        GameEvents.OnDialogueEvent -= HandleDialogueEvent;
    }

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

    private void OnMouseDown()
    {
        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canInteract = gameplayState == GameplayState.Playing && uiState == UIState.None;
        if (!canInteract) return;

        if (!canOpenPuzzle || isOpen) return;

        OpenPuzzle();
    }

    private void OpenPuzzle()
    {
        isOpen = true;

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            puzzlePanel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameEvents.RequestUIStateChange?.Invoke(UIState.Choice);
        onPuzzleOpened?.Invoke();
    }

    public void ClosePuzzle()
    {
        isOpen = false;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        GameEvents.RequestUIStateChange?.Invoke(UIState.None);
        onPuzzleClosed?.Invoke();
    }
}
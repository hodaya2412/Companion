using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : MonoBehaviour, IPuzzlePanelOwner
{
    public PuzzlePanelController puzzlePanelController;
    public UnityEvent onPuzzleOpened;
    public UnityEvent onPuzzleClosed;
    public ItemGiver itemGiver;

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
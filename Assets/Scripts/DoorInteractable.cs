using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : MonoBehaviour
{
    public GameObject puzzlePanel;
    public UnityEvent onPuzzleOpened;
    public UnityEvent onPuzzleClosed;

    private bool isOpen = false;
    private bool canOpenPuzzle = false;

    void OnEnable()
    {
        GameEvents.OnDialogueEvent += HandleDialogueEvent;
    }

    void OnDisable()
    {
        GameEvents.OnDialogueEvent -= HandleDialogueEvent;
    }

    private void HandleDialogueEvent(DialogueAction action)
    {
        
        if (action is UnlockPuzzleAction)
        {
            canOpenPuzzle = true;
        }
    }

    void OnMouseDown()
    {
        if (!canOpenPuzzle || isOpen) return;
        OpenPuzzle();
    }

    void OpenPuzzle()
    {
        isOpen = true;
        puzzlePanel.SetActive(true);
        puzzlePanel.transform.SetAsLastSibling();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        onPuzzleOpened?.Invoke();
    }

    public void ClosePuzzle()
    {
        isOpen = false;
        puzzlePanel.SetActive(false);
        onPuzzleClosed?.Invoke();
    }
}

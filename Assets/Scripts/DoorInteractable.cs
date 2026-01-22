using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : MonoBehaviour
{
    public GameObject puzzlePanel;

    [Header("Events")]
    public UnityEvent onPuzzleOpened;
    public UnityEvent onPuzzleClosed;

    private bool isOpen = false;

    void OnMouseDown()
    {
        if (isOpen) return;
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

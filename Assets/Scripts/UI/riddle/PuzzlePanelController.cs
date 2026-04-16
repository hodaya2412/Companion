using UnityEngine;

public class PuzzlePanelController : MonoBehaviour
{
    public GameObject panel;

    private IPuzzlePanelOwner currentOwner;

    public void Open(IPuzzlePanelOwner owner = null)
    {
        currentOwner = owner;

        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameEvents.RequestUIStateChange?.Invoke(UIState.Choice);
    }

    public void RequestClose()
    {
        if (panel != null)
            panel.SetActive(false);

        GameEvents.RequestUIStateChange?.Invoke(UIState.None);

        currentOwner?.OnPuzzlePanelClosed();
        currentOwner = null;
    }
}
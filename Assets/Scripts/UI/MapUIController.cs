using UnityEngine;

public class MapUIController : MonoBehaviour
{
    public GameObject mapPanel;

    public void ToggleMap()
    {
        if (mapPanel == null) return;

        bool isActive = mapPanel.activeSelf;
        mapPanel.SetActive(!isActive);

        if (!isActive)
            GameStateManager.Instance.SetState(GameState.Map);
        else
            GameStateManager.Instance.SetState(GameState.Playing);
    }
}

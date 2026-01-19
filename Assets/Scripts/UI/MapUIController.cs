using UnityEngine;

public class MapUIController : MonoBehaviour
{
    public GameObject mapPanel;

    public void ToggleMap()
    {
        if (mapPanel == null) return;
        mapPanel.SetActive(!mapPanel.activeSelf);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public GameObject tooltipPanel;    
    public TextMeshProUGUI tooltipText; 

    [Header("Content")]
    public string locationName = "טירת הלחשים";
    [TextArea] public string description = "לחץ כדי להיכנס לממלכה";

    [Header("Cursor")]
    public Texture2D hoverCursor;

    [SerializeField] private float tooltipOffsetY = 70f;
    [SerializeField] private float cursorHotspotX = 16f;
    [SerializeField] private float cursorHotspotY = 16f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = $"{locationName}\n<size=80%>{description}</size>";

            
            tooltipPanel.transform.position = transform.position + new Vector3(0, tooltipOffsetY, 0);
        }

        
        if (hoverCursor != null)
            Cursor.SetCursor(hoverCursor, new Vector2(cursorHotspotX, cursorHotspotY), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
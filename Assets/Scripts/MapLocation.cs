using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public GameObject tooltipPanel;    // גררי לכאן את ה-TooltipPanel מה-Hierarchy
    public TextMeshProUGUI tooltipText; // גררי לכאן את ה-Text (TMP) שבתוך הפאנל

    [Header("Content")]
    public string locationName = "טירת הלחשים";
    [TextArea] public string description = "לחץ כדי להיכנס לממלכה";

    [Header("Cursor")]
    public Texture2D hoverCursor; // אופציונלי: תמונת סמן של יד

    public void OnPointerEnter(PointerEventData eventData)
    {
        // הצגת ה-Tooltip ועדכון הטקסט
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = $"{locationName}\n<size=80%>{description}</size>";

            // מיקום הבועה מעל הכפתור
            tooltipPanel.transform.position = transform.position + new Vector3(0, 70, 0);
        }

        // שינוי סמן העכבר ליד
        if (hoverCursor != null)
            Cursor.SetCursor(hoverCursor, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // הסתרת ה-Tooltip והחזרת הסמן לרגיל
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
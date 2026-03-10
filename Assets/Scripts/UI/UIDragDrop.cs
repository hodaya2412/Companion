using UnityEngine;
using UnityEngine.EventSystems; // חשוב מאוד לאינטראקציה של UI

public class UIDragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // מוצא את הקאנבס שבו האבן נמצאת כדי לחשב מרחקים נכון
        canvas = GetComponentInParent<Canvas>();

        // אם אין Canvas Group על האובייקט, נוסיף אחד אוטומטית
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // כשלוחצים על האבן, היא קופצת לקדמת ה-UI (מעל האבנים האחרות)
        transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // הופך את האבן לקצת שקופה בזמן הגרירה
        canvasGroup.alpha = 0.6f;
        // מאפשר לקרן הלייזר (Raycast) לעבור דרך האבן כדי שהיא לא תחסום את העכבר
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // הזזה של האבן לפי תנועת העכבר/אצבע
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // מחזיר את האבן למצב רגיל
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
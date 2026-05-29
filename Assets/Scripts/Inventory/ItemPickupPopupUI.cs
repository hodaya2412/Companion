using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // נדרש כדי להשתמש ברכיב ה-Image של ה-UI

public class ItemPickupPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform container;

    private void OnEnable()
    {
        GameEvents.OnItemAdded += ShowPopup;
    }

    private void OnDisable()
    {
        GameEvents.OnItemAdded -= ShowPopup;
    }

    private void ShowPopup(InventoryItemData item, int amount)
    {
        // יצירת הפופ-אפ מתוך ה-Prefab בתוך ה-Container
        GameObject popup = Instantiate(popupPrefab, container);

        // 1. עדכון הטקסט של החפץ והכמות
        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = $"+{amount} {item.displayName}";
        }

        // 2. עדכון האייקון של החפץ בתוך ה-Prefab
        // הקוד מחפש ילד בשם "ItemIcon" בתוך הפופ-אפ שנוצר
        Transform iconTransform = popup.transform.Find("ItemIcon");
        if (iconTransform != null)
        {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
            }
        }

        // הפעלת הטיימר להשמדה עצמית כעבור 2 שניות
        StartCoroutine(DestroyAfterTime(popup, 4f));
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public Button button;

    public event Action<InventoryItemData> Clicked;

    private InventoryItemData item;

    private void Awake()
    {
        if (icon != null) icon.raycastTarget = false;
        if (amountText != null) amountText.raycastTarget = false;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (item != null) Clicked?.Invoke(item);
    }

    public void Set(InventoryItemData newItem, int amount)
    {
        item = newItem;
        bool hasItem = (item != null);

        // --- התיקון כאן: הסתרת או הצגת כל הסלוט ---
        gameObject.SetActive(hasItem);

        if (!hasItem) return; // אם אין פריט, אין צורך להמשיך לעדכן ויזואלית

        // עדכון האייקון
        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = (icon.sprite != null);
        }

        // עדכון טקסט הכמות
        if (amountText != null)
        {
            amountText.text = (amount > 1) ? $"x{amount}" : "";
        }

        if (button != null)
            button.interactable = true;
    }
}
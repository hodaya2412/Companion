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
        Debug.Log("Slot button clicked!");
        if (item != null) Clicked?.Invoke(item);
    }


    public void Set(InventoryItemData newItem, int amount)
    {
        item = newItem;
        bool hasItem = item != null;

        if (icon != null)
        {
            icon.sprite = hasItem ? item.icon : null;
            icon.enabled = hasItem && icon.sprite != null;
        }

        if (amountText != null)
            amountText.text = (hasItem && amount > 1) ? $"x{amount}" : "";

        if (button != null)
            button.interactable = hasItem;
    }
}

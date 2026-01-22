using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRowUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;

    public void Set(InventoryItemData item, int amount)
    {
        if (nameText != null) nameText.text = item != null ? item.displayName : "Unknown";
        if (amountText != null) amountText.text = amount > 1 ? $"x{amount}" : "";
        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.enabled = (icon.sprite != null);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public InventoryItemData item;
    public int amount;

    public InventorySlot(InventoryItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> slots = new();
    public IReadOnlyList<InventorySlot> Slots => slots;

    private void NotifyChanged()
    {
        GameEvents.OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return slots.Exists(s => s.item != null && s.item.itemId == itemId && s.amount > 0);
    }

    public bool AddItem(InventoryItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        if (!item.stackable && HasItem(item.itemId))
            return false;

        if (item.stackable)
        {
            var slot = slots.Find(s => s.item == item);
            if (slot != null)
            {
                slot.amount = Mathf.Min(slot.amount + amount, item.maxStack);
                NotifyChanged();
                return true;
            }
        }

        slots.Add(new InventorySlot(item, amount));
        NotifyChanged();
        return true;
    }

    public bool RemoveItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;

        var slot = slots.Find(s => s.item != null && s.item.itemId == itemId);
        if (slot == null) return false;

        slot.amount -= amount;

        if (slot.amount <= 0)
            slots.Remove(slot);

        NotifyChanged();
        return true;
    }
}

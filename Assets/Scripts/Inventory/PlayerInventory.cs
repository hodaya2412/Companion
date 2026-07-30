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
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Player Inventory")]
public class PlayerInventory : ScriptableObject
{
    [SerializeField] private List<InventorySlot> slots = new();
    public IReadOnlyList<InventorySlot> Slots => slots;

    [SerializeField] private int minValidAmountThreshold = 0;
    private void NotifyChanged()
    {
        GameEvents.OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return slots.Exists(s => s.item != null && s.item.itemId == itemId && s.amount > minValidAmountThreshold);
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
                int oldAmount = slot.amount;
                slot.amount = Mathf.Min(slot.amount + amount, item.maxStack);
                int addedAmount = slot.amount - oldAmount;

                NotifyChanged();
                GameEvents.OnItemAdded?.Invoke(item, addedAmount);
                return true;
            }
        }

        slots.Add(new InventorySlot(item, amount));
        NotifyChanged();
        GameEvents.OnItemAdded?.Invoke(item, amount);
        return true;
    }

    public bool RemoveItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;

        var slot = slots.Find(s => s.item != null && s.item.itemId == itemId);
        if (slot == null) return false;

        slot.amount -= amount;

        if (slot.amount <= minValidAmountThreshold)
            slots.Remove(slot);
        
        NotifyChanged();
        return true;

    }

    public void ResetInventory()
    {
        slots.Clear();
        NotifyChanged();
    }
}

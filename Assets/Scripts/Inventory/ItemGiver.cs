using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemGrant
{
    public InventoryItemData item;
    public int amount = 1;
}

public class ItemGiver : MonoBehaviour
{
    public PlayerInventory inventory;
    public bool giveOnlyOnce = true;
    public List<ItemGrant> grants = new();

    private bool alreadyGiven;

    public void Give()
    {
        if (giveOnlyOnce && alreadyGiven) return;

        // הסרנו את ה-FindFirstObjectByType כי ה-inventory כבר משויך מלמעלה
        if (inventory == null)
        {
            Debug.LogError("ItemGiver: Inventory file is missing!");
            return;
        }

        bool gaveAnything = false;

        foreach (var g in grants)
        {
            if (g.item == null || g.amount <= 0) continue;
            if (inventory.AddItem(g.item, g.amount))
                gaveAnything = true;
        }

        if (gaveAnything)
            alreadyGiven = true;
    }
}
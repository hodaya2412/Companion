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
    public bool giveOnlyOnce = true;
    public List<ItemGrant> grants = new();

    private bool alreadyGiven;

    public void Give()
    {
        if (giveOnlyOnce && alreadyGiven) return;

        var inv = FindFirstObjectByType<PlayerInventory>();
        if (inv == null) return;

        bool gaveAnything = false;

        foreach (var g in grants)
        {
            if (g.item == null || g.amount <= 0) continue;
            if (inv.AddItem(g.item, g.amount))
                gaveAnything = true;
        }

        if (gaveAnything)
            alreadyGiven = true;
    }
}

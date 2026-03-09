using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Add Item")]
public class AddItemAction : DialogueAction
{
    public InventoryItemData itemToAdd;
    public int amount = 1;
    public PlayerInventory inventory;

    public override void Execute()
    {
        if (inventory == null)
        {
            var inventories = Resources.FindObjectsOfTypeAll<PlayerInventory>();

            if (inventories.Length > 0)
                inventory = inventories[0];
        }

        if (inventory != null && itemToAdd != null)
        {
            inventory.AddItem(itemToAdd, amount);
            Debug.Log($"[Action] Added {amount}x {itemToAdd.itemId} to inventory.");
        }
        else
        {
            Debug.LogWarning("AddItemAction: Inventory or Item is missing.");
        }
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Add Inventory Item", fileName = "AddInventoryItemAction")]
public class AddInventoryItemAction : DialogueAction
{
    public PlayerInventory inventory;
    public InventoryItemData item;
    public int amount = 1;

    public override void Execute()
    {
        if (inventory == null || item == null)
        {
            Debug.LogError("AddInventoryItemAction: Missing inventory or item.");
            return;
        }

        inventory.AddItem(item, amount);
        Debug.Log($"Added item: {item.itemId}");
    }
}
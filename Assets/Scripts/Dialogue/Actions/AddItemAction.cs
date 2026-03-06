using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Add Item")]
public class AddItemAction : DialogueAction
{
    public InventoryItemData itemToAdd;
    public int amount = 1;
    public PlayerInventory inventory;

    public override void Execute()
    {
        if (inventory == null) inventory = Resources.FindObjectsOfTypeAll<PlayerInventory>()[0];

        if (inventory != null && itemToAdd != null)
        {
            inventory.AddItem(itemToAdd, amount);
            Debug.Log($"[Action] Added {amount}x {itemToAdd.itemId} to inventory.");
        }
    }
}
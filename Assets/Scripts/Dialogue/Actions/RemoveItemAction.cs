using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Remove Item")]
public class RemoveItemAction : DialogueAction
{
    public string itemId;
    public PlayerInventory inventory;

    public override void Execute()
    {
        if (inventory == null)
        {
            var inventories = Resources.FindObjectsOfTypeAll<PlayerInventory>();

            if (inventories.Length > 0)
                inventory = inventories[0];
        }

        if (inventory != null)
        {
            inventory.RemoveItem(itemId, 1);
            Debug.Log($"[Action] Removed {itemId} from inventory.");
        }
        else
        {
            Debug.LogWarning("RemoveItemAction: Inventory not found.");
        }
    }
}
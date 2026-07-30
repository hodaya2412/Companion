using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Remove Item")]
public class RemoveItemAction : DialogueAction
{
    public string itemId;
    public PlayerInventory inventory;

    [SerializeField] private int minInventoriesCount = 0;
    [SerializeField] private int targetInventoryIndex = 0;
    [SerializeField] private int defaultRemoveAmount = 1;

    public override void Execute()
    {
        if (inventory == null)
        {
            var inventories = Resources.FindObjectsOfTypeAll<PlayerInventory>();

            if (inventories.Length > minInventoriesCount)
                inventory = inventories[targetInventoryIndex];
        }

        if (inventory != null)
        {
            inventory.RemoveItem(itemId, defaultRemoveAmount);
            Debug.Log($"[Action] Removed {itemId} from inventory.");
        }
        else
        {
            Debug.LogWarning("RemoveItemAction: Inventory not found.");
        }
    }
}
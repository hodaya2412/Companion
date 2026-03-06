using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Remove Item")]
public class RemoveItemAction : DialogueAction
{
    public string itemId;
    public PlayerInventory inventory;

    public override void Execute()
    {
        if (inventory == null) inventory = Resources.FindObjectsOfTypeAll<PlayerInventory>()[0];
        inventory.RemoveItem(itemId, 1);
        Debug.Log($"[Action] Removed {itemId} from inventory.");
    }
}
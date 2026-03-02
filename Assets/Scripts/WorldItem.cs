using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public string worldItemId;   // מזהה ייחודי
    public InventoryItemData itemData;
    public PlayerInventory inventory;

    public void Pickup()
    {
        if (inventory != null && itemData != null)
        {
            bool added = inventory.AddItem(itemData, 1);

            if (added)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
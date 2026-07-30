using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public string worldItemId;   
    public InventoryItemData itemData;
    public PlayerInventory inventory;

    [Header("Optional persistent flag")]
    public string collectedFlag;

    private void Start()
    {
        
        if (!string.IsNullOrEmpty(collectedFlag) &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(collectedFlag))
        {
            Debug.Log($"[Persistent State] {worldItemId} already collected via flag. Disabling world object.");
            gameObject.SetActive(false);
            return;
        }

        
        if (inventory != null && itemData != null)
        {
            if (inventory.HasItem(itemData.itemId))
            {
                Debug.Log($"[Persistent State] {worldItemId} is already in inventory. Disabling world object.");
                gameObject.SetActive(false);
            }
        }
    }

    public void Pickup()
    {
        if (inventory != null && itemData != null)
        {
            bool added = inventory.AddItem(itemData, 1);

            if (added)
            {
                
                if (!string.IsNullOrEmpty(collectedFlag) &&
                    GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.SetFlag(collectedFlag, true);
                }

                if (WorldItemRegistry.Instance != null)
                {
                    WorldItemRegistry.Instance.Unregister(worldItemId);
                }

                GameEvents.OnItemPickedUp?.Invoke(worldItemId);

                gameObject.SetActive(false);

                Debug.Log($"[WorldItem] {worldItemId} picked up and registered as collected.");
            }
        }
    }
}
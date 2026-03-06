using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public string worldItemId;   // מזהה ייחודי
    public InventoryItemData itemData;
    public PlayerInventory inventory;

    private void Start()
    {
        // בדיקה אם הספר כבר נמצא בתיק של השחקן ברגע שהסצנה עולה
        if (inventory != null && itemData != null)
        {
            // אנחנו משתמשים ב-itemId מתוך ה-itemData כדי לבדוק בתיק
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
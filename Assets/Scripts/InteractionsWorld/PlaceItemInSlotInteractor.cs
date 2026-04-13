using UnityEngine;

public class PlaceItemInSlotInteractor : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private InventoryItemData requiredItem;

    [Header("Persistent World State")]
    [SerializeField] private string placedFlag;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool consumeItem = true;

    private bool inRange;
    private bool alreadyPlaced;

    private void OnEnable()
    {
        GameEvents.OnItemClicked += HandleItemClicked;
    }

    private void OnDisable()
    {
        GameEvents.OnItemClicked -= HandleItemClicked;
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(placedFlag) &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(placedFlag))
        {
            alreadyPlaced = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            inRange = false;
    }

    private void HandleItemClicked(InventoryItemData clickedItem)
    {
        if (!inRange || alreadyPlaced)
            return;

        if (clickedItem == null || requiredItem == null)
            return;

        if (clickedItem.itemId != requiredItem.itemId)
            return;

        TryPlaceItem();
    }

    private void TryPlaceItem()
    {
        if (inventory == null || requiredItem == null)
        {
            Debug.LogWarning("PlaceItemInSlotInteractor: Missing inventory or required item.");
            return;
        }

        if (!inventory.HasItem(requiredItem.itemId))
        {
            Debug.Log("Player does not have the required item.");
            return;
        }

        if (consumeItem)
        {
            bool removed = inventory.RemoveItem(requiredItem.itemId, 1);
            if (!removed)
            {
                Debug.LogWarning("Failed to remove item from inventory.");
                return;
            }
        }

        if (!string.IsNullOrEmpty(placedFlag) && GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetFlag(placedFlag, true);
        }

        alreadyPlaced = true;
        GameEvents.OnMirrorPlacedInForest?.Invoke();
    }
}
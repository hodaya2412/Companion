using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro; // ✅ בשביל TMP_Text

public class InventoryUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;                  // InventoryPanel
    public Transform contentParent;           // ScrollView/Viewport/Content
    public InventorySlotUI slotPrefab;        // Prefab של סלוט

    [Header("Fixed Slots Settings")]
    public int slotCount = 100;

    [Header("Runtime Data")]
    public PlayerInventory inventory;
    private ItemCategory? selectedCategory = null;

    [Header("Puzzle Settings")]
    public string puzzleItemId = "Item_Puzzle_Door01"; // ה-ID המדויק של הפריט
    public GameObject puzzlePanel;

    [Header("Quests Settings")]
    public GameObject questPanel;
    public TMP_Text questDescriptionText;     // ✅ זה היה חסר!

    [Header("Equipment")]
    public PlayerEquipment playerEquipment;

    private readonly List<InventorySlotUI> slotUIs = new();

    private void Start()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();

        if (playerEquipment == null)
            playerEquipment = FindFirstObjectByType<PlayerEquipment>();

        GameEvents.OnInventoryChanged += Refresh;

        BuildFixedSlots();

        if (panel != null) panel.SetActive(false);
        Refresh();
    }

    private void OnDestroy()
    {
        GameEvents.OnInventoryChanged -= Refresh;

        foreach (var ui in slotUIs)
        {
            if (ui != null) ui.Clicked -= OnSlotClicked;
        }
    }

    public void SetCategory(int categoryIndex)
    {
        if (categoryIndex == -1)
            selectedCategory = null;
        else
            selectedCategory = (ItemCategory)categoryIndex;

        Refresh();
    }

    public void Toggle()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing &&
            GameStateManager.Instance.CurrentState != GameState.Inventory) return;

        bool isActive = !panel.activeSelf;
        panel.SetActive(isActive);
        GameStateManager.Instance.SetState(isActive ? GameState.Inventory : GameState.Playing);

        if (isActive) Refresh();
    }

    private void BuildFixedSlots()
    {
        slotUIs.Clear();

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(contentParent.GetChild(i).gameObject);
        }

        if (slotPrefab == null)
        {
            Debug.LogError("Slot Prefab is missing on InventoryUIController!");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(slotPrefab, contentParent);
            ui.Set(null, 0);
            ui.gameObject.SetActive(false);
            ui.Clicked += OnSlotClicked;
            slotUIs.Add(ui);
        }
    }

    public void Refresh()
    {
        if (inventory == null) return;

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotUIs[i] != null)
                slotUIs[i].gameObject.SetActive(false);
        }

        var filteredItems = inventory.Slots
            .Where(s => s.item != null && (!selectedCategory.HasValue || s.item.category == selectedCategory.Value))
            .ToList();

        for (int i = 0; i < filteredItems.Count && i < slotUIs.Count; i++)
        {
            if (slotUIs[i] != null)
            {
                slotUIs[i].gameObject.SetActive(true);
                slotUIs[i].Set(filteredItems[i].item, filteredItems[i].amount);
            }
        }
    }

    private void OnSlotClicked(InventoryItemData item)
    {
        if (item == null)
        {
            Debug.Log("CLICKED SLOT! item is NULL (empty slot)");
            return;
        }

        Debug.Log($"CLICKED SLOT! ItemId: {item.itemId} | Category: {item.category}");

        // ✅ Weapon
        if (item.category == ItemCategory.Weapon)
        {
            Debug.Log("THIS IS A WEAPON!");

            if (playerEquipment != null)
            {
                if (playerEquipment.EquippedWeapon == item)
                {
                    Debug.Log("WEAPON ALREADY EQUIPPED -> UNEQUIP");
                    playerEquipment.UnequipWeapon();
                }
                else
                {
                    Debug.Log("EQUIP / SWITCH WEAPON");
                    playerEquipment.EquipWeapon(item);
                }
            }
            else
            {
                Debug.LogWarning("playerEquipment is NULL");
            }

            return;
        }

        // ✅ Quest
        if (item.category == ItemCategory.Quest)
        {
            Debug.Log("THIS IS A QUEST!");

            if (questPanel != null)
            {
                questPanel.SetActive(true);
                questPanel.transform.SetAsLastSibling();

                if (questDescriptionText != null)
                    questDescriptionText.text = item.description;
            }
            else
            {
                Debug.LogWarning("questPanel is NULL (not assigned in inspector)");
            }

            return;
        }

        // ✅ Puzzle
        if (item.itemId == puzzleItemId)
        {
            Debug.Log("THIS IS A PUZZLE ITEM!");

            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(true);
                puzzlePanel.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogWarning("puzzlePanel is NULL (not assigned in inspector)");
            }

            return;
        }

        Debug.Log("NOT QUEST, NOT PUZZLE, AND NOT WEAPON");
    }
    public void CloseSpecificPuzzle()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);

            if (GameStateManager.Instance != null)
                GameStateManager.Instance.SetState(GameState.Playing);
        }
    }

    public void CloseQuestPanel()
    {
        if (questPanel != null)
            questPanel.SetActive(false);

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetState(GameState.Inventory);
    }
}
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Transform contentParent;
    public InventorySlotUI slotPrefab;

    [Header("Fixed Slots Settings")]
    public int slotCount = 100;

    [Header("Runtime Data")]
    public PlayerInventory inventory;
    private ItemCategory? selectedCategory = null;
    private GameState currentGameState;

    [Header("Puzzle Settings")]
    public string puzzleItemId = "Item_Puzzle_Door01";
    public GameObject puzzlePanel;

    [Header("Quests Settings")]
    public GameObject questPanel;
    public TMP_Text questDescriptionText;

    private readonly List<InventorySlotUI> slotUIs = new();

    private void OnEnable()
    {
        GameEvents.OnInventoryChanged += Refresh;
        GameEvents.OnStateChanged += HandleStateChanged;
        GameEvents.OnItemClicked += OnSlotClicked; // ה-UI מאזין לעצמו כדי לנהל פאנלים פנימיים
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryChanged -= Refresh;
        GameEvents.OnStateChanged -= HandleStateChanged;
        GameEvents.OnItemClicked -= OnSlotClicked;
    }

    private void Start()
    {
        if (inventory == null) inventory = FindFirstObjectByType<PlayerInventory>();
        BuildFixedSlots();
        if (panel != null) panel.SetActive(false);
        Refresh();
    }

    private void HandleStateChanged(GameState newState) => currentGameState = newState;

    public void SetCategory(int categoryIndex)
    {
        selectedCategory = (categoryIndex == -1) ? null : (ItemCategory)categoryIndex;
        Refresh();
    }

    public void Toggle()
    {
        if (currentGameState != GameState.Playing && currentGameState != GameState.Inventory) return;

        bool isActive = !panel.activeSelf;
        panel.SetActive(isActive);

        // שליחת בקשה לשינוי מצב במקום גישה ישירה ל-Instance
        GameEvents.RequestStateChange?.Invoke(isActive ? GameState.Inventory : GameState.Playing);

        if (isActive) Refresh();
    }

    private void BuildFixedSlots()
    {
        slotUIs.Clear();
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(contentParent.GetChild(i).gameObject);

        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(slotPrefab, contentParent);
            ui.Set(null, 0);
            ui.gameObject.SetActive(false);
            slotUIs.Add(ui);
        }
    }

    public void Refresh()
    {
        if (inventory == null) return;

        foreach (var slot in slotUIs) slot.gameObject.SetActive(false);

        var filteredItems = inventory.Slots
            .Where(s => s.item != null && (!selectedCategory.HasValue || s.item.category == selectedCategory.Value))
            .ToList();

        for (int i = 0; i < filteredItems.Count && i < slotUIs.Count; i++)
        {
            slotUIs[i].gameObject.SetActive(true);
            slotUIs[i].Set(filteredItems[i].item, filteredItems[i].amount);
        }
    }

    private void OnSlotClicked(InventoryItemData item)
    {
        if (item == null) return;

        // טיפול בפאנלים פנימיים של ה-UI
        if (item.category == ItemCategory.Quest)
        {
            if (questPanel != null)
            {
                questPanel.SetActive(true);
                questPanel.transform.SetAsLastSibling();
                if (questDescriptionText != null) questDescriptionText.text = item.description;
            }
        }
        else if (item.itemId == puzzleItemId)
        {
            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(true);
                puzzlePanel.transform.SetAsLastSibling();
            }
        }
        // שים לב: אין כאן טיפול בנשק! ה-PlayerEquipment יטפל בזה.
    }

    public void CloseSpecificPuzzle()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        GameEvents.RequestStateChange?.Invoke(GameState.Playing);
    }

    public void CloseQuestPanel()
    {
        if (questPanel != null) questPanel.SetActive(false);
    }
}
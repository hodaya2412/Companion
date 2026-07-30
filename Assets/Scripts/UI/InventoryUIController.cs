using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class InventoryUIController : MonoBehaviour, IPuzzlePanelOwner
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

    private GameplayState currentGameplayState;
    private UIState currentUIState;

    [Header("Puzzle Settings")]
    public string puzzleItemId = "Item_Puzzle_Door01";
    public PuzzlePanelController puzzlePanelController;


    [Header("Quests Settings")]
    public GameObject questPanel;
    public TMP_Text questDescriptionText;

    [SerializeField] private int unassignedCategoryIndex = -1;

    private readonly List<InventorySlotUI> slotUIs = new();

    private Dictionary<string, ItemCategory> selectedItems = new();
    private void OnEnable()
    {
        GameEvents.OnInventoryChanged += Refresh;
        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged += HandleUIStateChanged;
        GameEvents.OnItemClicked += OnSlotClicked;
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryChanged -= Refresh;
        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged -= HandleUIStateChanged;
        GameEvents.OnItemClicked -= OnSlotClicked;
    }

    private void Start()
    {
        if (inventory == null) inventory = FindFirstObjectByType<PlayerInventory>();

        BuildFixedSlots();

        if (panel != null) panel.SetActive(false);

        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        currentUIState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        Refresh();
    }

    private void HandleGameplayStateChanged(GameplayState newState) => currentGameplayState = newState;
    private void HandleUIStateChanged(UIState newState) => currentUIState = newState;

    public void SetCategory(int categoryIndex)
    {
        selectedCategory = (categoryIndex == unassignedCategoryIndex) ? null : (ItemCategory)categoryIndex;
        Refresh();
    }

    public void Toggle()
    {
        bool gameplayAllowsInventory =
            currentGameplayState == GameplayState.Playing ||
            currentGameplayState == GameplayState.Combat;

        bool uiAllowsInventory =
            currentUIState == UIState.None ||
            currentUIState == UIState.Inventory;

        if (!gameplayAllowsInventory || !uiAllowsInventory)
            return;

        bool isActive = !panel.activeSelf;
        panel.SetActive(isActive);

        GameEvents.RequestUIStateChange?.Invoke(isActive ? UIState.Inventory : UIState.None);

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
            ui.Set(null, 0,false);
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
            bool isSelected = selectedItems.ContainsKey(filteredItems[i].item.itemId);
            slotUIs[i].Set(filteredItems[i].item, filteredItems[i].amount, isSelected);
        }
    }

    private void OnSlotClicked(InventoryItemData item)
    {
        if (item == null) return;

     
        if (selectedItems.ContainsKey(item.itemId))
        {
            selectedItems.Remove(item.itemId);
            Refresh();

            if (item.category == ItemCategory.Quest && questPanel != null)
                questPanel.SetActive(false);
            else if (item.itemId == puzzleItemId && puzzlePanelController != null)
                puzzlePanelController.RequestClose();

            return;
        }

        
        if (item.category == ItemCategory.Weapon)
        {
            RemoveSelectionByCategory(ItemCategory.Weapon);
        }

       
        selectedItems[item.itemId] = item.category;
        Refresh();

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
            if (puzzlePanelController != null)
                puzzlePanelController.Open(this);
        }
    }

   
    private void RemoveSelectionByCategory(ItemCategory category)
    {
        var toRemove = selectedItems
            .Where(kv => kv.Value == category)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var id in toRemove)
            selectedItems.Remove(id);
    }
    public void OnPuzzlePanelClosed()
    {
        if (panel != null && panel.activeSelf)
            GameEvents.RequestUIStateChange?.Invoke(UIState.Inventory);
        else
            GameEvents.RequestUIStateChange?.Invoke(UIState.None);
    }
}
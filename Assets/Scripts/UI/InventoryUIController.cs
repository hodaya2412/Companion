using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;                 // InventoryPanel
    public Transform contentParent;          // ScrollView/Viewport/Content
    public InventorySlotUI slotPrefab;       // Prefab של סלוט

    [Header("Fixed Slots")]
    public int slotCount = 100;

    [Header("Runtime")]
    public PlayerInventory inventory;

    [Header("Puzzle Open (Scene Reference)")]
    public string puzzleItemId = "Item_Puzzle_Door01"; // לשים כאן את ה-ID המדויק של פריט החידה
    public GameObject puzzlePanel;                    // לגרור את puzzlePanel מה-Hierarchy (בסצנה)

    private readonly List<InventorySlotUI> slotUIs = new();

    private void Start()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();

        GameEvents.OnInventoryChanged += Refresh;

        BuildFixedSlots();

        if (panel != null) panel.SetActive(false);
        Refresh();
    }

    private void OnDestroy()
    {
        GameEvents.OnInventoryChanged -= Refresh;

        // ניקוי מאזינים (לא חובה אבל טוב)
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotUIs[i] != null)
                slotUIs[i].Clicked -= OnSlotClicked;
        }
    }

    public void Toggle()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing &&
            GameStateManager.Instance.CurrentState != GameState.Inventory)
            return;

        bool isActive = panel.activeSelf;
        panel.SetActive(!isActive);

        if (!isActive)
            GameStateManager.Instance.SetState(GameState.Inventory);
        else
            GameStateManager.Instance.SetState(GameState.Playing);

        Refresh();
    }

    private void BuildFixedSlots()
    {
        if (contentParent == null || slotPrefab == null) return;

        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(slotPrefab, contentParent);
            ui.Set(null, 0);

            // מאזינים ללחיצה על סלוט
            ui.Clicked += OnSlotClicked;

            slotUIs.Add(ui);
        }
    }

    private void OnSlotClicked(InventoryItemData item)
    {
        Debug.Log($"CLICKED: {item.displayName} | id={item.itemId} | target={puzzleItemId} | panelNull={puzzlePanel == null}");
        if (item == null) return;

        // אם זה הפריט של החידה — פותחים את הפאנל מהסצנה
        if (!string.IsNullOrEmpty(puzzleItemId) && item.itemId == puzzleItemId && puzzlePanel != null)
        {
            puzzlePanel.transform.SetAsLastSibling(); // מעל האינוונטורי
            puzzlePanel.SetActive(true);
        }
        else
        {
            Debug.Log($"Clicked item: {item.displayName} ({item.itemId})");
        }
    }

    private void Refresh()
    {
        if (inventory == null) return;

        // 1) לאפס
        for (int i = 0; i < slotUIs.Count; i++)
            slotUIs[i].Set(null, 0);

        // 2) למלא לפי מה שיש
        int index = 0;
        foreach (var slot in inventory.Slots)
        {
            if (slot.item == null) continue;
            if (index >= slotUIs.Count) break;

            slotUIs[index].Set(slot.item, slot.amount);
            index++;
        }
    }
}

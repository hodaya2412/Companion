using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public GameObject puzzlePanel;                    // הפאנל של החידה בסצנה

    private readonly List<InventorySlotUI> slotUIs = new();

    private void Start()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();

        GameEvents.OnInventoryChanged += Refresh;

        // בניית הסלוטים פעם אחת בלבד כדי למנוע כפילויות
        BuildFixedSlots();

        if (panel != null) panel.SetActive(false);
        Refresh();
    }

    private void OnDestroy()
    {
        GameEvents.OnInventoryChanged -= Refresh;
        // ניקוי מאזינים למניעת זליגת זיכרון
        foreach (var ui in slotUIs)
        {
            if (ui != null) ui.Clicked -= OnSlotClicked;
        }
    }

    // פונקציה עבור כפתורי הלשוניות (Puzzles = 0, Weapons = 5)
    public void SetCategory(int categoryIndex)
    {
        // כאן אנחנו רק מחליפים קטגוריה - זה לא פותח את החידה!
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
        // 1. קודם כל מנקים את הרשימה הישנה מהזיכרון
        slotUIs.Clear();

        // 2. מוחקים פיזית כל מה שנשאר ב-Content
        // השתמשנו בלופ הפוך כדי למנוע בעיות באינדקסים בזמן מחיקה
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            // DestroyImmediate מוחק את האובייקט עכשיו ולא מחכה לסוף הפריים
            DestroyImmediate(contentParent.GetChild(i).gameObject);
        }

        // 3. יוצרים את 100 הסלוטים החדשים מה-Prefab
        if (slotPrefab == null)
        {
            Debug.LogError("Slot Prefab is missing on InventoryUIController!");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(slotPrefab, contentParent);
            ui.Set(null, 0); // מאתחל כריק
            ui.gameObject.SetActive(false); // מכבה כברירת מחדל
            ui.Clicked += OnSlotClicked;
            slotUIs.Add(ui);
        }
    }

    public void Refresh()
    {
        if (inventory == null) return;

        // 1. כיבוי כל הסלוטים הקיימים ברשימה
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (slotUIs[i] != null) // בדיקת בטיחות
                slotUIs[i].gameObject.SetActive(false);
        }

        // 2. סינון פריטים
        var filteredItems = inventory.Slots
            .Where(s => s.item != null && (!selectedCategory.HasValue || s.item.category == selectedCategory.Value))
            .ToList();

        // 3. הצגת הפריטים המסוננים
        for (int i = 0; i < filteredItems.Count && i < slotUIs.Count; i++)
        {
            if (slotUIs[i] != null)
            {
                slotUIs[i].gameObject.SetActive(true);
                slotUIs[i].Set(filteredItems[i].item, filteredItems[i].amount);
            }
        }
    }

    // פונקציה שנקראת רק כשלוחצים פיזית על אייקון בתוך התיק
    private void OnSlotClicked(InventoryItemData item)
    {
        if (item == null) return;

        // רק אם ה-ID של הפריט שלחצנו עליו הוא של החידה - נפתח הפאנל
        if (item.itemId == puzzleItemId && puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            puzzlePanel.transform.SetAsLastSibling();
        }
    }
}
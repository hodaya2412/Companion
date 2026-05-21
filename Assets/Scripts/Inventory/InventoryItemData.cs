using UnityEngine;

public enum ItemCategory
{
    Puzzle, KeyItem, Consumable, Quest, Misc, Weapon
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string displayName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;
    public Sprite selectedIcon;

    [Header("Behavior")]
    public ItemCategory category = ItemCategory.Misc;
    public bool stackable = false;
    public int maxStack = 99;

    [Header("Puzzle / UI (optional)")]
    public GameObject puzzlePanel; // גוררים לפה את ה-Panel של החידה (אם זה פריט חידה)

    [Header("Weapon Animation")]
    public WeaponAnimationType weaponAnimationType = WeaponAnimationType.None;
}

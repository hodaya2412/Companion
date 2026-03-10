using UnityEngine;
using System;

public class PlayerEquipment : MonoBehaviour
{
    public static PlayerEquipment Instance { get; private set; }

    [SerializeField] private InventoryItemData equippedWeapon;
    public InventoryItemData EquippedWeapon => equippedWeapon;

    // אירוע מקומי שמאפשר ל-Combat לדעת על שינוי בנשק
    public event Action<InventoryItemData> OnWeaponEquipped;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        // חשוב: שם האירוע ב-GameEvents שלך הוא Clicked
        GameEvents.OnItemClicked += HandleItemClicked;
    }

    private void OnDisable()
    {
        GameEvents.OnItemClicked -= HandleItemClicked;
    }

    private void HandleItemClicked(InventoryItemData item)
    {
        // אם הפריט שנלחץ אינו נשק, אנחנו לא עושים כלום
        if (item == null || item.category != ItemCategory.Weapon) return;

        if (equippedWeapon == item)
            UnequipWeapon();
        else
            EquipWeapon(item);
    }

    public void EquipWeapon(InventoryItemData weaponItem)
    {
        equippedWeapon = weaponItem;
        Debug.Log($"Equipped weapon: {equippedWeapon.displayName}");
        OnWeaponEquipped?.Invoke(equippedWeapon);
    }

    public void UnequipWeapon()
    {
        equippedWeapon = null;
        Debug.Log("Weapon unequipped");
        OnWeaponEquipped?.Invoke(null);
    }

    public bool HasWeaponEquipped() => equippedWeapon != null;
}
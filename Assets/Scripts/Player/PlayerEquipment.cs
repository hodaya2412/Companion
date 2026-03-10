using UnityEngine;
using System;

public class PlayerEquipment : MonoBehaviour
{
    public static PlayerEquipment Instance { get; private set; }

    [Header("Equipped Weapon")]
    [SerializeField] private InventoryItemData equippedWeapon;

    public InventoryItemData EquippedWeapon => equippedWeapon;

    public event Action<InventoryItemData> OnWeaponEquipped;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EquipWeapon(InventoryItemData weaponItem)
    {
        if (weaponItem == null) return;

        if (weaponItem.category != ItemCategory.Weapon)
        {
            Debug.LogWarning($"Tried to equip non-weapon item: {weaponItem.displayName}");
            return;
        }

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

    public bool HasWeaponEquipped()
    {
        return equippedWeapon != null;
    }
}
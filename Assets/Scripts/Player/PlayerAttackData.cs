using UnityEngine;

public struct PlayerAttackData
{
    public GameObject attacker;
    public float damage;
    public bool hasWeaponEquipped;
    public InventoryItemData equippedWeapon;

    public PlayerAttackData(GameObject attacker, float damage, bool hasWeaponEquipped, InventoryItemData equippedWeapon)
    {
        this.attacker = attacker;
        this.damage = damage;
        this.hasWeaponEquipped = hasWeaponEquipped;
        this.equippedWeapon = equippedWeapon;
    }
}
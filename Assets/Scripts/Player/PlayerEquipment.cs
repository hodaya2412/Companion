using UnityEngine;
using System;

public class PlayerEquipment : MonoBehaviour
{
    public static PlayerEquipment Instance { get; private set; }

    [Header("Data Persistence")]
    public PlayerStateSO playerState;

    // מחזיר תמיד את הנשק השמור בתוך ה-PlayerStateSO (מונע התאפסות במעבר סצנה)
    public InventoryItemData EquippedWeapon => (playerState != null) ? playerState.currentWeaponItem : null;

    // אירוע מקומי למי שעדיין רוצה להאזין לשינויים בזמן אמת במשחק
    public event Action<InventoryItemData> OnWeaponEquipped;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // קריטי: מעדכן את האנימטור של השחקן החדש מיד כשהסצנה עולה
        UpdateAnimator();
    }

    private void OnEnable()
    {
        GameEvents.OnItemClicked += HandleItemClicked;
    }

    private void OnDisable()
    {
        GameEvents.OnItemClicked -= HandleItemClicked;
    }

    private void HandleItemClicked(InventoryItemData item)
    {
        if (item == null || item.category != ItemCategory.Weapon) return;

        if (EquippedWeapon == item)
            UnequipWeapon();
        else
            EquipWeapon(item);
    }

    public void EquipWeapon(InventoryItemData weaponItem)
    {
        if (playerState != null)
        {
            playerState.isArmed = true;
            playerState.weaponType = (int)weaponItem.weaponAnimationType;
            playerState.currentWeaponItem = weaponItem; // נשמר ישירות ב-SO ששורד סצנות
        }

        UpdateAnimator();
        OnWeaponEquipped?.Invoke(weaponItem);
    }

    public void UnequipWeapon()
    {
        if (playerState != null)
        {
            playerState.isArmed = false;
            playerState.weaponType = 0;
            playerState.currentWeaponItem = null;
        }

        UpdateAnimator();
        OnWeaponEquipped?.Invoke(null);
    }

    // מסנכרן את האנימטור הנוכחי בסצנה עם מה ששמור ב-State
    public void UpdateAnimator()
    {
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null && playerState != null)
        {
            animator.SetBool("IsArmed", playerState.isArmed);
            animator.SetInteger("WeaponType", playerState.weaponType);
        }
    }

    // בדיקה אמינה ב-100% כי היא קוראת ישירות מה-SO
    public bool HasWeaponEquipped()
    {
        if (playerState == null) return false;
        return playerState.isArmed && playerState.currentWeaponItem != null;
    }
}
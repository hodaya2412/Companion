using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerState", menuName = "OS/Player State")]
public class PlayerStateSO : ScriptableObject
{
    [Header("Movement Data")]
    public float lastMoveX = 0;
    public float lastMoveY = -1; 

    [Header("Equipment Data")]
    public bool isArmed = false;
    public int weaponType = 0;

    public InventoryItemData currentWeaponItem;

    
    public void ResetState()
    {
        lastMoveX = 0;
        lastMoveY = -1;
        isArmed = false;
        weaponType = 0;
    }
}
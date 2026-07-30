using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayer;

    [Header("Attackable Objects")]
    public LayerMask attackableLayer;

    [Header("Unarmed Attack")]
    public float unarmedDamage = 10f;
    public float unarmedRange = 1.5f;

    [Header("Weapon Attack")]
    public float weaponDamage = 25f;
    public float weaponRange = 2.2f;

    [Header("General Combat")]
    public float attackCooldown = 0.5f;

    private Animator animator; 

    private InputActions inputActions;
    private float lastAttackTime = -999f;

    private GameplayState currentGameplayState;
    private UIState currentUIState;
    private bool hasWeaponEquipped;


    
    [SerializeField] private float attackBoxHalfWidth = 0.7f;
    [SerializeField] private float attackBoxHalfHeight = 1f;
    [SerializeField] private float rangeMultiplier = 0.5f;

    private void Awake()
    {
        inputActions = new InputActions();
        if (attackPoint == null) attackPoint = transform;
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += OnAttackPerformed;

        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged += HandleUIStateChanged;

        if (PlayerEquipment.Instance != null)
        {
            PlayerEquipment.Instance.OnWeaponEquipped += HandleWeaponChanged;
            hasWeaponEquipped = PlayerEquipment.Instance.HasWeaponEquipped();
        }

        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        currentUIState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Disable();

        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged -= HandleUIStateChanged;

        if (PlayerEquipment.Instance != null)
            PlayerEquipment.Instance.OnWeaponEquipped -= HandleWeaponChanged;
    }

    private void HandleGameplayStateChanged(GameplayState newState) => currentGameplayState = newState;
    private void HandleUIStateChanged(UIState newState) => currentUIState = newState;

    private void HandleWeaponChanged(InventoryItemData weapon)
    {
        hasWeaponEquipped = (weapon != null);
        Debug.Log($"[PlayerCombat] Weapon changed. Equipped = {hasWeaponEquipped}, weapon = {(weapon != null ? weapon.itemId : "NULL")}");
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("[PlayerCombat] Attack input received");
        TryAttack();
    }

    private void TryAttack()
    {
        bool gameplayAllowsAttack =
            currentGameplayState == GameplayState.Playing ||
            currentGameplayState == GameplayState.Combat;

        bool uiBlocksAttack =
            currentUIState == UIState.Dialogue ||
            currentUIState == UIState.Map ||
            currentUIState == UIState.Choice;

        Debug.Log($"[PlayerCombat] TryAttack | gameplay={currentGameplayState} | ui={currentUIState} | hasWeapon={hasWeaponEquipped}");

        if (!gameplayAllowsAttack)
        {
            Debug.Log("[PlayerCombat] Attack blocked: gameplay state");
            return;
        }

        if (uiBlocksAttack)
        {
            Debug.Log("[PlayerCombat] Attack blocked: UI state");
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
        {
            Debug.Log("[PlayerCombat] Attack blocked: cooldown");
            return;
        }

        lastAttackTime = Time.time;
        if (animator != null)
        {
            
            animator.SetBool("IsAttacking", true);
            animator.SetTrigger("Attack");
        }

        float damage = hasWeaponEquipped ? weaponDamage : unarmedDamage;
        float range = hasWeaponEquipped ? weaponRange : unarmedRange;

        Debug.Log($"[PlayerCombat] Performing attack | damage={damage} | range={range}");

        PerformAttackOverlap(damage, range);
    }

    private void PerformAttackOverlap(float damage, float range)
    {
        Vector3 center = attackPoint.position + transform.forward * range * rangeMultiplier;
        Vector3 halfExtents = new Vector3(attackBoxHalfWidth, attackBoxHalfHeight, range * rangeMultiplier);

        Debug.Log($"[PlayerCombat] Overlap center={center}, halfExtents={halfExtents}");

        Collider[] enemyHits = Physics.OverlapBox(center, halfExtents, transform.rotation, enemyLayer, QueryTriggerInteraction.Collide);
        Debug.Log($"[PlayerCombat] enemyHits count = {enemyHits.Length}");

        foreach (Collider hit in enemyHits)
        {
            GameEvents.OnEnemyHit?.Invoke(hit.gameObject, damage);
        }

        Collider[] attackableHits = Physics.OverlapBox(center, halfExtents, transform.rotation, attackableLayer);
        Debug.Log($"[PlayerCombat] attackableHits count = {attackableHits.Length}");

        PlayerAttackData attackData = new PlayerAttackData(
            gameObject,
            damage,
            hasWeaponEquipped,
            PlayerEquipment.Instance != null ? PlayerEquipment.Instance.EquippedWeapon : null
        );

        HashSet<IPlayerAttackReceiver> alreadyHitReceivers = new HashSet<IPlayerAttackReceiver>();

        foreach (Collider hit in attackableHits)
        {
            Debug.Log($"[PlayerCombat] Hit collider: {hit.name} | Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");

            IPlayerAttackReceiver receiver = hit.GetComponent<IPlayerAttackReceiver>();

            if (receiver == null)
                receiver = hit.GetComponentInParent<IPlayerAttackReceiver>();

            if (receiver == null)
                receiver = hit.GetComponentInChildren<IPlayerAttackReceiver>();

            if (receiver != null && !alreadyHitReceivers.Contains(receiver))
            {
                alreadyHitReceivers.Add(receiver);
                Debug.Log($"[PlayerCombat] Found receiver on: {((MonoBehaviour)receiver).name}");
                receiver.ReceivePlayerAttack(attackData);
            }
            else if (receiver == null)
            {
                Debug.Log($"[PlayerCombat] No receiver found for: {hit.name}");
            }
        }
    }
    
    public void OnAttackEnded()
    {
        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
            Debug.Log("[PlayerCombat] Attack ended, returning to idle.");
        }
    }
}
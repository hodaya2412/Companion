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

    // מאפיין (Property) שבודק בזמן אמת מול ה-Equipment וה-SO בכל רגע נתון
    private bool HasWeaponEquipped => PlayerEquipment.Instance != null && PlayerEquipment.Instance.HasWeaponEquipped();

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

        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        currentUIState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Disable();

        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged -= HandleUIStateChanged;
    }

    private void HandleGameplayStateChanged(GameplayState newState) => currentGameplayState = newState;
    private void HandleUIStateChanged(UIState newState) => currentUIState = newState;

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

        // שליפת הנתון האמיתי והעדכני בזמן הלחיצה
        bool isArmed = HasWeaponEquipped;

        Debug.Log($"[PlayerCombat] TryAttack | gameplay={currentGameplayState} | ui={currentUIState} | hasWeapon={isArmed}");

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

        // שימוש בנתון הקיים (isArmed)
        float damage = isArmed ? weaponDamage : unarmedDamage;
        float range = isArmed ? weaponRange : unarmedRange;

        Debug.Log($"[PlayerCombat] Performing attack | damage={damage} | range={range}");

        PerformAttackOverlap(damage, range, isArmed);
    }

    private void PerformAttackOverlap(float damage, float range, bool isArmed)
    {
        Vector3 center = attackPoint.position + transform.forward * range * 0.5f;
        Vector3 halfExtents = new Vector3(0.7f, 1f, range * 0.5f);

        Debug.Log($"[PlayerCombat] Overlap center={center}, halfExtents={halfExtents}");

        Collider[] enemyHits = Physics.OverlapBox(center, halfExtents, transform.rotation, enemyLayer);
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
            isArmed,
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) attackPoint = transform;

        // עדכון הטווח בגיזמוס מול המצב האמיתי ב-SO
        float range = HasWeaponEquipped ? weaponRange : unarmedRange;
        Vector3 center = attackPoint.position + transform.forward * range * 0.5f;
        Vector3 halfExtents = new Vector3(0.7f, 1f, range * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
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
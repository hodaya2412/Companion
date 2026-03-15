using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayer;

    [Header("Unarmed Attack")]
    public float unarmedDamage = 10f;
    public float unarmedRange = 1.5f;

    [Header("Weapon Attack")]
    public float weaponDamage = 25f;
    public float weaponRange = 2.2f;

    [Header("General Combat")]
    public float attackCooldown = 0.5f;

    private InputActions inputActions;
    private float lastAttackTime = -999f;

    private GameplayState currentGameplayState;
    private UIState currentUIState;
    private bool hasWeaponEquipped;

    private void Awake()
    {
        inputActions = new InputActions();
        if (attackPoint == null) attackPoint = transform;
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
    private void HandleWeaponChanged(InventoryItemData weapon) => hasWeaponEquipped = (weapon != null);

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
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

        if (!gameplayAllowsAttack || uiBlocksAttack) return;

        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        float damage = hasWeaponEquipped ? weaponDamage : unarmedDamage;
        float range = hasWeaponEquipped ? weaponRange : unarmedRange;

        PerformAttackOverlap(damage, range);
    }

    private void PerformAttackOverlap(float damage, float range)
    {
        Vector3 center = attackPoint.position + transform.forward * range * 0.5f;
        Vector3 halfExtents = new Vector3(0.7f, 1f, range * 0.5f);

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            transform.rotation,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            GameEvents.OnEnemyHit?.Invoke(hit.gameObject, damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) attackPoint = transform;

        float range = hasWeaponEquipped ? weaponRange : unarmedRange;
        Vector3 center = attackPoint.position + transform.forward * range * 0.5f;
        Vector3 halfExtents = new Vector3(0.7f, 1f, range * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
    }
}
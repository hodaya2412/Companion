using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public PlayerEquipment playerEquipment;

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

    private void Awake()
    {
        inputActions = new InputActions();

        if (playerEquipment == null)
            playerEquipment = GetComponent<PlayerEquipment>();

        if (attackPoint == null)
            attackPoint = transform;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Disable();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        bool hasWeapon = playerEquipment != null && playerEquipment.HasWeaponEquipped();
        float damage = hasWeapon ? weaponDamage : unarmedDamage;
        float range = hasWeapon ? weaponRange : unarmedRange;

        Debug.Log(hasWeapon
            ? $"Weapon attack! Damage: {damage}, Range: {range}"
            : $"Unarmed attack! Damage: {damage}, Range: {range}");

        Vector3 center = attackPoint.position + transform.forward * range * 0.5f;
        Vector3 halfExtents = new Vector3(0.7f, 1f, range * 0.5f);

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            transform.rotation,
            enemyLayer
        );

        Debug.Log("Hits found: " + hits.Length);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                Debug.Log("Sending hit event to: " + enemy.gameObject.name);
                GameEvents.OnEnemyHit?.Invoke(enemy.gameObject, damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) attackPoint = transform;

        bool hasWeapon = playerEquipment != null && playerEquipment.HasWeaponEquipped();
        float range = hasWeapon ? weaponRange : unarmedRange;

        Vector3 center = attackPoint.position + transform.forward * range * 0.5f;
        Vector3 halfExtents = new Vector3(0.7f, 1f, range * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
    }
}
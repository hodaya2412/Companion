using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackCoordinator : MonoBehaviour
{
    public static EnemyAttackCoordinator Instance { get; private set; }

    [Header("Attack Limits")]
    [SerializeField] private int maxConcurrentAttackers = 2;

    [Header("Attack Distances")]
    [SerializeField] private float frontOffset = 1.4f;
    [SerializeField] private float sideOffset = 1.8f;
    [SerializeField] private float rearOffset = 1.8f;

    [SerializeField] private float forwardSqrMagnitudeThreshold = 0.001f;

    private readonly HashSet<EnemyBrain> activeAttackers = new HashSet<EnemyBrain>();
    private readonly Dictionary<EnemyBrain, AttackRole> enemyRoles = new Dictionary<EnemyBrain, AttackRole>();

    private Transform player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void OnEnable()
    {
        GameEvents.OnCombatReset += ResetAll;
    }

    private void OnDisable()
    {
        GameEvents.OnCombatReset -= ResetAll;
    }

    public bool TryReserveAttackSlot(EnemyBrain enemy)
    {
        if (enemy == null) return false;

        if (activeAttackers.Contains(enemy))
            return true;

        if (activeAttackers.Count >= maxConcurrentAttackers)
            return false;

        AttackRole role = GetBestRoleForEnemy(enemy);
        if (role == AttackRole.None)
            return false;

        activeAttackers.Add(enemy);
        enemyRoles[enemy] = role;

        
        enemy.SetAttackRole(role);

        return true;
    }

    public void ReleaseAttackSlot(EnemyBrain enemy)
    {
        if (enemy == null) return;

        activeAttackers.Remove(enemy);
        enemyRoles.Remove(enemy);
    }

    public void ResetAll()
    {
        activeAttackers.Clear();
        enemyRoles.Clear();
    }

    private AttackRole GetBestRoleForEnemy(EnemyBrain enemy)
    {
        bool frontTaken = enemyRoles.ContainsValue(AttackRole.Front);
        bool leftTaken = enemyRoles.ContainsValue(AttackRole.LeftFlank);
        bool rightTaken = enemyRoles.ContainsValue(AttackRole.RightFlank);
        bool rearTaken = enemyRoles.ContainsValue(AttackRole.RearFlank);

      
        if (enemy.PrefersFlank())
        {
            if (!rearTaken) return AttackRole.RearFlank;
            if (!leftTaken) return AttackRole.LeftFlank;
            if (!rightTaken) return AttackRole.RightFlank;
            if (!frontTaken) return AttackRole.Front;

            return AttackRole.None;
        }

        if (!frontTaken)
            return AttackRole.Front;

        if (!leftTaken)
            return AttackRole.LeftFlank;

        return AttackRole.None;
    }

    public Vector3 GetAttackPosition(EnemyBrain enemy)
    {
        if (player == null || enemy == null)
            return enemy != null ? enemy.transform.position : Vector3.zero;

        if (!enemyRoles.TryGetValue(enemy, out AttackRole role))
            return player.position;

        return CalculateAttackPosition(role);
    }

    private Vector3 CalculateAttackPosition(AttackRole role)
    {
        Vector3 playerPos = player.position;
        Vector3 playerForward = player.forward;
        playerForward.y = 0f;

        if (playerForward.sqrMagnitude < forwardSqrMagnitudeThreshold)
            playerForward = Vector3.forward;

        playerForward.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, playerForward).normalized;

        switch (role)
        {
            case AttackRole.LeftFlank:
                return playerPos - side * sideOffset;

            case AttackRole.RightFlank:
                return playerPos + side * sideOffset;

            case AttackRole.RearFlank:
                return playerPos - playerForward * rearOffset;

            case AttackRole.Front:
            default:
                return playerPos + playerForward * frontOffset;
        }
    }
}
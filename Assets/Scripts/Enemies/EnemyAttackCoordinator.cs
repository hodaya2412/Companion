using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackCoordinator : MonoBehaviour
{
    public static EnemyAttackCoordinator Instance { get; private set; }

    [Header("Attack Limits")]
    [SerializeField] private int maxConcurrentAttackers = 2;

    private readonly HashSet<EnemyBrain> activeAttackers = new HashSet<EnemyBrain>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

        activeAttackers.Add(enemy);
        return true;
    }

    public void ReleaseAttackSlot(EnemyBrain enemy)
    {
        if (enemy == null) return;
        activeAttackers.Remove(enemy);
    }

    public void ResetAll()
    {
        activeAttackers.Clear();
    }
}
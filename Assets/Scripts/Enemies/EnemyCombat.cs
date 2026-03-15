using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackDamage = 10f;

    [Header("Cooldown Settings")]
    public float minAttackCooldown = 1.2f;
    public float maxAttackCooldown = 2f;

    private float nextAttackTime;

    private void Start()
    {
        SetNextAttackTime();
    }

    private void Update()
    {
        if (playerTarget == null) return;
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            SetNextAttackTime();
        }
    }

    private void AttackPlayer()
    {
        Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage");
        GameEvents.OnPlayerHit?.Invoke(attackDamage);
    }

    private void SetNextAttackTime()
    {
        float cooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        nextAttackTime = Time.time + cooldown;
    }
}
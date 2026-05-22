using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;

    [Header("Attack Settings")]
    public float attackRange = 2.7f;
    public float attackDamage = 10f;

    [Header("Cooldown Settings")]
    public float minAttackCooldown = 1.2f;
    public float maxAttackCooldown = 2f;

    private float nextAttackTime;
    private GameplayState currentGameplayState;
   
    [Header("Tutorial Integration")]
    public TutorialEventTrigger combatTutorial;

    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
    }

    private void OnDisable()
    {
        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
    }

    private void Start()
    {
        SetNextAttackTime();
    }

    private void HandleGameplayStateChanged(GameplayState newState)
    {
        currentGameplayState = newState;
        if (newState == GameplayState.Combat)
        {
            if (combatTutorial != null)
            {
                combatTutorial.TriggerShow();
            }
        }
    }

    public bool CanAttack()
    {
        if (playerTarget == null) return false;
        if (currentGameplayState != GameplayState.Combat) return false;
        if (Time.time < nextAttackTime) return false;

        float distance = Vector3.Distance(transform.position, playerTarget.position);
        return distance <= attackRange;
    }

    public void TryAttack()
    {
        if (!CanAttack()) return;

        AttackPlayer();
        SetNextAttackTime();
    }

    private void AttackPlayer()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        Debug.Log($"{gameObject.name} attacked player for {attackDamage} damage");
        GameEvents.OnPlayerHit?.Invoke(attackDamage);
    }

    private void SetNextAttackTime()
    {
        float cooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        nextAttackTime = Time.time + cooldown;
    }
}
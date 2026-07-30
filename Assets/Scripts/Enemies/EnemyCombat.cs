using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;
    [SerializeField] private Animator animator;

    
    private EnemyBrain brain;

    [Header("Attack Settings")]
    public float attackRange;
    public float attackDamage;

    [Header("Cooldown Settings")]
    public float minAttackCooldown;
    public float maxAttackCooldown;

    private float nextAttackTime;
    private GameplayState currentGameplayState;

    [Header("Tutorial Integration")]
    public TutorialEventTrigger combatTutorial;


    [SerializeField] private float initialNextAttackTime = 0f;
    [SerializeField] private float fallbackPlayerAttackAllowance = 1.0f;

    private void Awake()
    {
      
        brain = GetComponent<EnemyBrain>();
    }

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
        nextAttackTime = initialNextAttackTime;
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
        if (playerTarget == null) return;

        AttackPlayer();
        SetNextAttackTime();
    }

    private void AttackPlayer()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Debug.Log($"{gameObject.name} started attack animation.");
    }

    public void ExecuteDamageEvent()
    {
        if (playerTarget == null) return;

        
        float allowance = (brain != null) ? brain.playerAttackAllowance : fallbackPlayerAttackAllowance;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

      
        if (distance <= attackRange + allowance)
        {
            PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                GameEvents.OnPlayerHit?.Invoke(attackDamage);
              
                Debug.Log($"[Hit!] {gameObject.name} dealt {attackDamage} damage to player via Animation Event.");
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} attacked, but player dodged out of range!");
        }
    }

    private void SetNextAttackTime()
    {
        float cooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        nextAttackTime = Time.time + cooldown;
    }
}
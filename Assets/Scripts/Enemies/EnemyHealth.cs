using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float destroyDelay = 1.2f;
    [SerializeField] private EnemyHitFlash hitFlash;


    [Header("Health")]
    public float maxHealth = 50f;
    public float currentHealth = 50f;

    [Header("Persistence (שמירה בין סצנות)")]
    [Tooltip("תני לכל אויב שם ייחודי ב-Inspector, למשל Bandit_1")]
    public string enemyID;

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnDied;

    [SerializeField] private float minHealthLimit = 0f;
    [SerializeField] private float minDamageThreshold = 0f;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, minHealthLimit, maxHealth);
    }

    private void Start()
    {
        
        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(enemyID))
        {
            if (GameStateManager.Instance.GetFlag("Dead_" + enemyID))
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyHit += HandleEnemyHit;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyHit -= HandleEnemyHit;
    }

    private void HandleEnemyHit(GameObject target, float damage)
    {
        if (target != gameObject) return;
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (damage <= minDamageThreshold) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, minHealthLimit, maxHealth);

        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (hitFlash != null)
        {
            hitFlash.Flash();
        }

        if (currentHealth > minHealthLimit && animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        if (currentHealth <= minHealthLimit)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= minDamageThreshold) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, minHealthLimit, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        
        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(enemyID))
        {
            GameStateManager.Instance.SetFlag("Dead_" + enemyID, true);
        }

     
        EnemyBrain brain = GetComponent<EnemyBrain>();
        if (brain != null)
        {
            brain.ReleaseAttackSlotIfNeeded();
        }

      
        CheckIfLastEnemy();

        OnDied?.Invoke();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, destroyDelay);
    }

    private void CheckIfLastEnemy()
    {
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

       
        if (enemies.Length <= 1)
        {
            if (GameStateManager.Instance != null)
            {
              
                GameStateManager.Instance.SetFlag("Forest_BanditsDefeated", true);
                Debug.Log("All enemies defeated! Permanent flag set.");
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 50f;
    public float currentHealth = 50f;

    [Header("Persistence (שמירה בין סצנות)")]
    [Tooltip("תני לכל אויב שם ייחודי ב-Inspector, למשל Bandit_1")]
    public string enemyID;

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnDied;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Start()
    {
        // בדיקה בטעינת הסצנה: אם האויב הזה כבר מת בעבר, נשמיד אותו מיד
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
        if (damage <= 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        // 1. שמירת המצב ב-GameStateManager כדי שלא יחזור בסצנה הבאה
        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(enemyID))
        {
            GameStateManager.Instance.SetFlag("Dead_" + enemyID, true);
        }

        // 2. שחרור ה-Slot ב-Coordinator כדי שאויב אחר יוכל לתקוף
        EnemyBrain brain = GetComponent<EnemyBrain>();
        if (brain != null)
        {
            brain.ReleaseAttackSlotIfNeeded();
        }

        // 3. בדיקה אם זה האויב האחרון כדי לחסום את אבן החידה
        CheckIfLastEnemy();

        OnDied?.Invoke();
        Destroy(gameObject);
    }

    private void CheckIfLastEnemy()
    {
        // מחפשים את כל מי שיש לו תגית Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // אם נשאר רק 1 (האויב הנוכחי שעדיין לא הושמד סופית), זה האחרון
        if (enemies.Length <= 1)
        {
            if (GameStateManager.Instance != null)
            {
                // מעדכנים את הדגל שחוסם את ה-PuzzleStone
                GameStateManager.Instance.SetFlag("Forest_BanditsDefeated", true);
                Debug.Log("All enemies defeated! Permanent flag set.");
            }
        }
    }
}
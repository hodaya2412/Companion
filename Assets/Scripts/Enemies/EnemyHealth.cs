using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 50f;
    public float currentHealth = 50f;

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnDied;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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
        OnDied?.Invoke();

        Destroy(gameObject);
    }
}
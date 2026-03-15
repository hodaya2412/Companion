using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Regen")]
    [Tooltip("כמה שניות אחרי פגיעה מתחילים רגנרציה")]
    public float regenDelay = 4f;

    [Tooltip("כמה חיים לשנייה מתחדש")]
    public float regenRate = 3f;

    [Tooltip("אם את רוצה שהרגנרציה תעבוד רק מעל 0")]
    public bool regenOnlyIfAlive = true;

    private float lastDamageTime;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = -999f;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerHit += HandlePlayerHit;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerHit -= HandlePlayerHit;
    }

    private void Start()
    {
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        HandleRegen();
    }

    private void HandlePlayerHit(float damage)
    {
        Debug.Log("PLAYER RECEIVED HIT EVENT: " + damage);
        TakeDamage(damage);
    }

    private void HandleRegen()
    {
        if (regenOnlyIfAlive && currentHealth <= 0f) return;

        if (Time.time >= lastDamageTime + regenDelay)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += regenRate * Time.deltaTime;
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}/{maxHealth}");

        lastDamageTime = Time.time;
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died");

        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
    }
}
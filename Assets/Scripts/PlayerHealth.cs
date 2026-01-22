using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent<float, float> OnHealthChanged; 

    private float lastDamageTime;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = -999f;
    }

    private void Update()
    {
        HandleRegen();
    }

    private void HandleRegen()
    {
        if (regenOnlyIfAlive && currentHealth <= 0f) return;

        // אם עבר מספיק זמן מאז הפגיעה האחרונה
        if (Time.time >= lastDamageTime + regenDelay)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += regenRate * Time.deltaTime; // עולה בהדרגה
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        lastDamageTime = Time.time; // עוצר רגנרציה ומתחיל דיליי מחדש
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        
        Debug.Log("Player Died");
    }
}

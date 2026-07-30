using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Regen")]
    public float regenDelay = 7f;

    public float regenRate = 1.5f;

    public bool regenOnlyIfAlive = true;

    [Header("Death / Respawn")]
  
    [SerializeField] private float deathDelay = 0.5f;
    [SerializeField] private Transform respawnPoint;

    [SerializeField] private float minHealthThreshold = 0f;
    [SerializeField] private float initialLastDamageTime = -999f;
    [SerializeField] private float respawnFadeInDelay = 0.2f;

    private float lastDamageTime;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, minHealthThreshold, maxHealth);
        lastDamageTime = initialLastDamageTime;
    }

    private void Start()
    {
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        HandleRegen();
    }


    private void HandleRegen()
    {
        if (regenOnlyIfAlive && currentHealth <= minHealthThreshold) return;
        if (isDead) return;

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
        if (isDead) return;
        if (damage <= minHealthThreshold) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, minHealthThreshold, maxHealth);

        Debug.Log($"Player took {damage} damage. Current HP: {currentHealth}/{maxHealth}");

        lastDamageTime = Time.time;
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= minHealthThreshold)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= minHealthThreshold) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, minHealthThreshold, maxHealth);

        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RestoreFullHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        lastDamageTime = initialLastDamageTime;
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = minHealthThreshold;
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("Player Died");

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        GameEvents.RequestUIStateChange?.Invoke(UIState.None);

        yield return new WaitForSeconds(deathDelay);

      
        if (SceneFader.Instance != null)
            yield return SceneFader.Instance.FadeOut();

       
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("Respawn point not set!");
        }

        
        RestoreFullHealth();

      
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
        GameEvents.OnCombatReset?.Invoke();

        yield return new WaitForSeconds(respawnFadeInDelay); 

        
        if (SceneFader.Instance != null)
            yield return SceneFader.Instance.FadeIn();
    }
}
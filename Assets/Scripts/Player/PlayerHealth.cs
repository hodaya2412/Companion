using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Regen")]
    public float regenDelay = 4f;

    public float regenRate = 3f;

    public bool regenOnlyIfAlive = true;

    [Header("Death / Respawn")]
    [SerializeField] private string castleSceneName = "Castle_Intro";
    [SerializeField] private float deathDelay = 0.5f;

    private float lastDamageTime;
    private bool isDead = false;

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
        if (isDead) return;

        Debug.Log("PLAYER RECEIVED HIT EVENT: " + damage);
        TakeDamage(damage);
    }

    private void HandleRegen()
    {
        if (regenOnlyIfAlive && currentHealth <= 0f) return;
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

    public void RestoreFullHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        lastDamageTime = -999f;
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0f;
        GameEvents.OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("Player Died");

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        GameEvents.RequestUIStateChange?.Invoke(UIState.None);

        yield return new WaitForSeconds(deathDelay);

        if (SceneFader.Instance != null)
        {
            yield return SceneFader.Instance.FadeOutAndLoad(castleSceneName);
        }
        else
        {
            SceneManager.LoadScene(castleSceneName);
        }

        // רק אחרי טעינת הסצנה:
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
        GameEvents.OnCombatReset?.Invoke();
        RestoreFullHealth();
    }
}
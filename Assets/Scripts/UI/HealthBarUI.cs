using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerHealth playerHealth;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            GameEvents.OnHealthChanged += UpdateBar;
            UpdateBar(playerHealth.currentHealth, playerHealth.maxHealth);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            GameEvents.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
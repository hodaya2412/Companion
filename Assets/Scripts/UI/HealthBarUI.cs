using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerHealth playerHealth;
    public Image fillImage;

    [SerializeField] private float smoothSpeed = 8f;

    private float targetHealth;
    private float maxHealth;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            maxHealth = playerHealth.maxHealth;
            targetHealth = playerHealth.currentHealth;

            slider.maxValue = maxHealth;
            slider.value = targetHealth;

            GameEvents.OnHealthChanged += OnHealthChanged;
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        targetHealth = current;
        maxHealth = max;
        slider.maxValue = maxHealth;
    }

    private void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetHealth, Time.deltaTime * smoothSpeed);

        if (Mathf.Abs(slider.value - targetHealth) < 0.01f)
            slider.value = targetHealth;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (fillImage == null || maxHealth <= 0) return;

        float percent = slider.value / maxHealth;

        if (percent > 0.6f)
            fillImage.color = Color.green;
        else if (percent > 0.3f)
            fillImage.color = Color.yellow;
        else
            fillImage.color = Color.red;
    }
}
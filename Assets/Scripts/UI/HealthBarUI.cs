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

    [SerializeField] private float minHealthThreshold = 0f;
    [SerializeField] private float sliderSnapThreshold = 0.01f;
    [SerializeField] private float greenHealthPercentThreshold = 0.6f;
    [SerializeField] private float yellowHealthPercentThreshold = 0.3f;
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

        if (Mathf.Abs(slider.value - targetHealth) < sliderSnapThreshold)
            slider.value = targetHealth;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (fillImage == null || maxHealth <= minHealthThreshold) return;

        float percent = slider.value / maxHealth;

        if (percent > greenHealthPercentThreshold)
            fillImage.color = Color.green;
        else if (percent > yellowHealthPercentThreshold)
            fillImage.color = Color.yellow;
        else
            fillImage.color = Color.red;
    }
}
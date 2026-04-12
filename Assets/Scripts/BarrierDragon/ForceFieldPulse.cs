using UnityEngine;

public class ForceFieldPulse : MonoBehaviour
{
    [Header("Texture Movement")]
    public float speedX = 0.2f;
    public float speedY = 0.1f;

    [Header("Pulse")]
    public float pulseSpeed = 2f;
    public float pulseStrength = 2f;
    public float alphaStrength = 0.5f;

    [Header("Base Colors")]
    public Color baseColor = new Color(1f, 0.1f, 0.1f, 1f);
    public Color damagedColor = Color.white;

    [Header("Break Progress")]
    [Range(0f, 1f)] public float breakProgress = 0f;
    public float emissionMultiplier = 2f;

    [Header("Hit Feedback")]
    public float hitFlashDecaySpeed = 6f;
    public float blockedHitFlashAmount = 0.35f;
    public float validHitFlashAmount = 0.7f;

    private Renderer rend;
    private Vector2 offset;
    private float hitFlash;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (rend == null) return;

        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;
        rend.material.mainTextureOffset = offset;

        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
        hitFlash = Mathf.MoveTowards(hitFlash, 0f, hitFlashDecaySpeed * Time.deltaTime);

        Color currentBase = Color.Lerp(baseColor, damagedColor, breakProgress);
        Color pulseColor = currentBase * (1f + pulse * pulseStrength);

        if (hitFlash > 0f)
        {
            pulseColor = Color.Lerp(pulseColor, Color.white, hitFlash);
        }

        pulseColor.a = Mathf.Lerp(0.3f, 0.05f, breakProgress) +
                       pulse * alphaStrength * Mathf.Lerp(1f, 0.5f, breakProgress);

        pulseColor.a = Mathf.Clamp01(pulseColor.a);

        rend.material.color = pulseColor;
        rend.material.SetColor("_EmissionColor", pulseColor * emissionMultiplier);
    }

    public void SetBreakProgress(float value)
    {
        breakProgress = Mathf.Clamp01(value);
    }

    public void TriggerBlockedHitFeedback()
    {
        hitFlash = Mathf.Max(hitFlash, blockedHitFlashAmount);
    }

    public void TriggerValidHitFeedback()
    {
        hitFlash = Mathf.Max(hitFlash, validHitFlashAmount);
    }
}
using UnityEngine;

public class ForceFieldPulse : MonoBehaviour
{
    public float speedX = 0.2f;
    public float speedY = 0.1f;

    public float pulseSpeed = 2f;
    public float pulseStrength = 2f;
    public float alphaStrength = 0.5f;

    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (rend == null) return;

        // תזוזת טקסטורה
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;
        rend.material.mainTextureOffset = offset;

        // Pulse
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        Color baseColor = new Color(1f, 0.1f, 0.1f, 1f);
        Color pulseColor = baseColor * (1f + pulse * pulseStrength);

        pulseColor.a = 0.3f + pulse * alphaStrength;

        rend.material.color = pulseColor;
        rend.material.SetColor("_EmissionColor", pulseColor * 2f);
    }
}
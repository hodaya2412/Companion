using UnityEngine;
using UnityEngine.UI;

public class MapAttentionSeeker : MonoBehaviour
{
    public Image glowImage; // גרירת ה-GlowEffect לכאן
    public float pulseSpeed = 3f;
    public float minAlpha = 0.1f;
    public float maxAlpha = 0.8f;

    private bool isGlowing = false;

    void Start()
    {
        if (glowImage != null)
        {
            // מוודא שהזוהר כבוי בהתחלה
            Color c = glowImage.color;
            c.a = 0;
            glowImage.color = c;
        }
    }

    public void StartGlow()
    {
        isGlowing = true;
    }

    public void StopGlow()
    {
        isGlowing = false;
        if (glowImage != null)
        {
            Color c = glowImage.color;
            c.a = 0;
            glowImage.color = c;
        }
    }

    void Update()
    {
        if (isGlowing && glowImage != null)
        {
            // חישוב שקיפות שמשתנה בזמן (בין min ל-max)
            float alpha = minAlpha + (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f * (maxAlpha - minAlpha);

            Color c = glowImage.color;
            c.a = alpha;
            glowImage.color = c;
        }
    }
}
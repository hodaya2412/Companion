using UnityEngine;
using UnityEngine.UI;

public class MapAttentionSeeker : MonoBehaviour
{
    public Image glowImage; 
    public float pulseSpeed = 3f;
    public float minAlpha = 0.1f;
    public float maxAlpha = 0.8f;

    [SerializeField] private float hiddenAlpha = 0f;
    [SerializeField] private float sineOffset = 1f;
    [SerializeField] private float sineDivisor = 2f;

    private bool isGlowing = false;

    void Start()
    {
        if (glowImage != null)
        {
           
            Color c = glowImage.color;
            c.a = hiddenAlpha;
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
            
            float alpha = minAlpha + (Mathf.Sin(Time.time * pulseSpeed) + sineOffset) / sineDivisor * (maxAlpha - minAlpha);

            Color c = glowImage.color;
            c.a = alpha;
            glowImage.color = c;
        }
    }
}
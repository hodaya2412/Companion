using System.Collections;
using UnityEngine;

public class ForestMirrorRevealController : MonoBehaviour
{
    [Header("Objects to reveal")]
    [SerializeField] private GameObject placedMirror;
    [SerializeField] private GameObject shadowProjection;

    [Header("Animated Rays")]
    [SerializeField] private AnimatedRayLine ray1;
    [SerializeField] private AnimatedRayLine ray2;
    [SerializeField] private AnimatedRayLine ray3;
    [SerializeField] private AnimatedRayLine ray4;

    [Header("Persistent State")]
    [SerializeField] private string placedFlag;

    [Header("Timing")]
    [SerializeField] private float delayBeforeShadow = 0.15f;
    [SerializeField] private float shadowFadeDuration = 0.6f;
    [SerializeField] private float shadowTargetAlpha = 0.47f;

    private bool activated;
    private MeshRenderer shadowRenderer;
    private Material shadowMaterial;

    private void Awake()
    {
        if (shadowProjection != null)
        {
            shadowRenderer = shadowProjection.GetComponent<MeshRenderer>();
            if (shadowRenderer != null)
            {
                shadowMaterial = shadowRenderer.material;
            }
        }
    }

    private void Start()
    {
        bool alreadyPlaced =
            !string.IsNullOrEmpty(placedFlag) &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(placedFlag);

        if (alreadyPlaced)
        {
            ApplySolvedStateInstant();
            activated = true;
        }
        else
        {
            ResetShadowAlpha();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnMirrorPlacedInForest += Reveal;
    }

    private void OnDisable()
    {
        GameEvents.OnMirrorPlacedInForest -= Reveal;
    }

    private void Reveal()
    {
        if (activated) return;
        activated = true;
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        if (placedMirror != null)
            placedMirror.SetActive(true);

        if (ray1 != null)
        {
            ray1.gameObject.SetActive(true);
            ray1.PlayBeam();
            yield return new WaitForSeconds(ray1.GrowDuration);
        }

        if (ray2 != null)
        {
            ray2.gameObject.SetActive(true);
            ray2.PlayBeam();
            yield return new WaitForSeconds(ray2.GrowDuration);
        }

        if (ray3 != null)
        {
            ray3.gameObject.SetActive(true);
            ray3.PlayBeam();
            yield return new WaitForSeconds(ray3.GrowDuration);
        }

        if (ray4 != null)
        {
            ray4.gameObject.SetActive(true);
            ray4.PlayBeam();
            yield return new WaitForSeconds(ray4.GrowDuration);
        }

        yield return new WaitForSeconds(delayBeforeShadow);

        if (shadowProjection != null)
        {
            shadowProjection.SetActive(true);
            yield return StartCoroutine(FadeInShadow());
        }
    }

    private IEnumerator FadeInShadow()
    {
        if (shadowMaterial == null)
            yield break;

        Color color = shadowMaterial.color;
        color.a = 0f;
        shadowMaterial.color = color;

        float t = 0f;
        while (t < shadowFadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / shadowFadeDuration);

            color.a = Mathf.Lerp(0f, shadowTargetAlpha, k);
            shadowMaterial.color = color;

            yield return null;
        }

        color.a = shadowTargetAlpha;
        shadowMaterial.color = color;
    }

    private void ResetShadowAlpha()
    {
        if (shadowMaterial == null) return;

        Color color = shadowMaterial.color;
        color.a = 0f;
        shadowMaterial.color = color;
    }

    private void ApplySolvedStateInstant()
    {
        if (placedMirror != null)
            placedMirror.SetActive(true);

        if (ray1 != null)
            ray1.gameObject.SetActive(true);

        if (ray2 != null)
            ray2.gameObject.SetActive(true);

        if (ray3 != null)
            ray3.gameObject.SetActive(true);

        if (ray4 != null)
            ray4.gameObject.SetActive(true);

        if (shadowProjection != null)
            shadowProjection.SetActive(true);

        if (shadowMaterial != null)
        {
            Color color = shadowMaterial.color;
            color.a = shadowTargetAlpha;
            shadowMaterial.color = color;
        }
    }
}
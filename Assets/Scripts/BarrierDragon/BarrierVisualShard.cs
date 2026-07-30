using System.Collections;
using UnityEngine;

public class BarrierVisualShard : MonoBehaviour
{
    [Header("Top To Bottom Fade")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float randomDelay = 0.03f;
    [SerializeField] private float minScaleY = 0.02f;
   
    [Header("Break Settings")]
    [SerializeField] private float positionOffsetFactor = 0.5f;
    [SerializeField] private float endEmissionMultiplier = 0.25f;
    [SerializeField] private float fullAlpha = 1f;

    private Renderer[] renderers;
    private Color[][] originalColors;

    private Vector3 startLocalScale;
    private Vector3 startLocalPosition;

    private Coroutine breakRoutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            originalColors[i] = new Color[mats.Length];

            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j].HasProperty("_Color"))
                    originalColors[i][j] = mats[j].color;
                else
                    originalColors[i][j] = Color.white;
            }
        }

        startLocalScale = transform.localScale;
        startLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        RestoreOriginalState();
        breakRoutine = null;
    }

    public void PlayBreak()
    {
        if (breakRoutine != null) return;
        breakRoutine = StartCoroutine(BreakRoutine());
    }

    private IEnumerator BreakRoutine()
    {
        float delay = Random.Range(0f, randomDelay);
        yield return new WaitForSeconds(delay);

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            float currentScaleY = Mathf.Lerp(startLocalScale.y, minScaleY, normalized);

            Vector3 scale = transform.localScale;
            scale.y = currentScaleY;
            transform.localScale = scale;

            float lostHeight = startLocalScale.y - currentScaleY;

            Vector3 pos = startLocalPosition;
            pos.y = startLocalPosition.y - (lostHeight * positionOffsetFactor);
            transform.localPosition = pos;

            float alpha = Mathf.Lerp(fullAlpha, 0f, normalized);

            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].HasProperty("_Color"))
                    {
                        Color c = originalColors[i][j];
                        c.a = alpha;
                        mats[j].color = c;
                    }

                    if (mats[j].HasProperty("_EmissionColor"))
                    {
                        Color emission = mats[j].GetColor("_EmissionColor");
                        emission *= Mathf.Lerp(fullAlpha, endEmissionMultiplier, normalized);
                        mats[j].SetColor("_EmissionColor", emission);
                    }
                }
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void RestoreOriginalState()
    {
        transform.localScale = startLocalScale;
        transform.localPosition = startLocalPosition;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;

            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j].HasProperty("_Color"))
                {
                    Color c = originalColors[i][j];
                    c.a = fullAlpha;
                    mats[j].color = c;
                }
            }
        }
    }
}
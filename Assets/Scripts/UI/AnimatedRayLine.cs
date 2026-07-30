using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AnimatedRayLine : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Animation")]
    [SerializeField] private float growDuration = 0.25f;

    [SerializeField] private int linePositionCount = 2;
    [SerializeField] private int startLineIndex = 0;
    [SerializeField] private int endLineIndex = 1;
    [SerializeField] private float initialAnimationProgress = 0f;

    public float GrowDuration => growDuration;

    private LineRenderer line;
    private Coroutine currentRoutine;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = linePositionCount;
        ResetLine();
    }

    public void SetPoints(Transform start, Transform end)
    {
        startPoint = start;
        endPoint = end;
        ResetLine();
    }

    public void PlayBeam()
    {
        if (startPoint == null || endPoint == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateBeam());
    }

    public void ResetLine()
    {
        if (line == null || startPoint == null) return;

        line.SetPosition(startLineIndex, startPoint.position);
        line.SetPosition(endLineIndex, startPoint.position);
    }

    private IEnumerator AnimateBeam()
    {
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        float t = initialAnimationProgress;
        line.SetPosition(startLineIndex, start);
        line.SetPosition(endLineIndex, start);

        while (t < growDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / growDuration);

            Vector3 currentEnd = Vector3.Lerp(start, end, k);
            line.SetPosition(startLineIndex, start);
            line.SetPosition(endLineIndex, currentEnd);

            yield return null;
        }

        line.SetPosition(startLineIndex, start);
        line.SetPosition(endLineIndex, end);
        currentRoutine = null;
    }
}
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    [SerializeField] private int linePositionCount = 2;
    [SerializeField] private int startLineIndex = 0;
    [SerializeField] private int endLineIndex = 1;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        line.positionCount = linePositionCount;
        line.SetPosition(startLineIndex, startPoint.position);
        line.SetPosition(endLineIndex, endPoint.position);
    }
}
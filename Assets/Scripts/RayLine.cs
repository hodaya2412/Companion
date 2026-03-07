using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayLine : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        line.positionCount = 2;
        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, endPoint.position);
    }
}
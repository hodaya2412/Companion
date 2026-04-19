using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("Float")]
    public float floatAmplitude = 0.15f;
    public float floatSpeed = 1.5f;

    [Header("Rotate")]
    public float rotationSpeed = 25f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0f, yOffset, 0f);

    }
}
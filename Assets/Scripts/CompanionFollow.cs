using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb; // isKinematic = true

    public float comfortableDistance = 1.5f;
    public float maxDistance = 4.5f;
    public float maxSpeed = 8f;

    private bool followEnabled = true;

    void OnEnable()
    {
        GameEvents.OnCompanionFollowEnabled += SetFollowEnabled;
    }

    void OnDisable()
    {
        GameEvents.OnCompanionFollowEnabled -= SetFollowEnabled;
    }

    void SetFollowEnabled(bool enabled)
    {
        followEnabled = enabled;
    }

    void FixedUpdate()
    {
        if (!followEnabled) return;
        if (player == null || rb == null) return;

        Vector3 toPlayer = player.position - rb.position;
        float distance = toPlayer.magnitude;

        if (distance <= comfortableDistance)
            return;

        float t = Mathf.InverseLerp(comfortableDistance, maxDistance, distance);
        float speed = Mathf.Lerp(0f, maxSpeed, t);

        Vector3 move = toPlayer.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}

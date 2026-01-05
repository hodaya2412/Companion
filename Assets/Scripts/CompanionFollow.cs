using UnityEngine;

public class CompanionFollowFixed : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb; // Rigidbody של היצור, isKinematic = true

    public float comfortableDistance = 1.5f;
    public float maxDistance = 4.5f;
    public float maxSpeed = 8f;

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        Vector3 toPlayer = player.position - rb.position;
        float distance = toPlayer.magnitude;

        if (distance <= comfortableDistance)
            return;

        // חישוב אחוז לפי מרחק
        float t = Mathf.InverseLerp(comfortableDistance, maxDistance, distance);
        float speed = Mathf.Lerp(0f, maxSpeed, t);

        Vector3 move = toPlayer.normalized * speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);
    }
}

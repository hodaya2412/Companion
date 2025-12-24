using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        Vector3 dir = cam.transform.forward;
        dir.y = 0f;
        transform.forward = dir.normalized;
    }
}

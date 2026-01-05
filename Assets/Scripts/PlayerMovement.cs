using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public Transform cam;   

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
    }

    void FixedUpdate()
    {
        float x = 0f;
        float y = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.downArrowKey.isPressed) y -= 1f;
            if (Keyboard.current.upArrowKey.isPressed) y += 1f;
        }

        Vector3 camForward = cam ? cam.forward : Vector3.forward;
        Vector3 camRight = cam ? cam.right : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camRight * x + camForward * y).normalized;

        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
       
    }

}

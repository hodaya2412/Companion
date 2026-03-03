using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public Transform cam;

    [Header("Input System")]
    InputActions inputAction; 

    private Rigidbody rb;
    private bool canMove = true;

    private Vector3 moveInput; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        inputAction = new InputActions();
        
    }

    void OnEnable()
    {
        if (inputAction != null)
        {
            inputAction.Player.Enable();
            inputAction.Player.Move.performed += OnMove;
            inputAction.Player.Move.canceled += OnMove;
        }

        GameEvents.OnStateChanged += HandleStateChanged;
    }


    void OnDisable()
    {
        if (inputAction != null)
        {
            inputAction.Player.Move.performed -= OnMove;
            inputAction.Player.Move.canceled -= OnMove;
            inputAction.Player.Disable();
        }
        if (GameStateManager.Instance != null)
        {
            Debug.Log($"[PlayerMovement] Unsubscribing from OnStateChanged. Object: {gameObject.name}");
            GameEvents.OnStateChanged -= HandleStateChanged;
        }

    }
    private void HandleStateChanged(GameState state)
    {
       
        SetMovementEnabled(state == GameState.Playing);
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector3>();
    }


    public void SetMovementEnabled(bool enabled)
    {
        // Player can move only in Playing mode OR if being guided
        GameState state = GameStateManager.Instance.CurrentState;
        canMove = (enabled && state == GameState.Playing) || state == GameState.BeingGuided;

        if (!canMove)
        {
            moveInput = Vector3.zero;
            rb.linearVelocity = Vector3.zero; // linearVelocity → velocity
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

     
        float x = moveInput.x;
        float z = moveInput.z;

        Vector3 camForward = cam ? cam.forward : Vector3.forward;
        Vector3 camRight = cam ? cam.right : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camRight * x + camForward * z).normalized;

        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }
}

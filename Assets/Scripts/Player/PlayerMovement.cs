using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public Transform cam;

    [Header("Input System")]
    private InputActions inputAction;

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
        GameEvents.OnDialogueStarted += HandleDialogueStarted;
        GameEvents.OnDialogueEnded += HandleDialogueEnded;
    }

    void OnDisable()
    {
        if (inputAction != null)
        {
            inputAction.Player.Move.performed -= OnMove;
            inputAction.Player.Move.canceled -= OnMove;
            inputAction.Player.Disable();
        }

        GameEvents.OnStateChanged -= HandleStateChanged;
        GameEvents.OnDialogueStarted -= HandleDialogueStarted;
        GameEvents.OnDialogueEnded -= HandleDialogueEnded;
    }

    private void HandleStateChanged(GameState state)
    {
        SetMovementEnabled(state == GameState.Playing);
    }

    private void HandleDialogueStarted()
    {
        SetMovementEnabled(false);
    }

    private void HandleDialogueEnded()
    {
        SetMovementEnabled(true);
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector3>();
    }

    public void SetMovementEnabled(bool enabled)
    {
        GameState state = GameEvents.RequestCurrentGameState?.Invoke() ?? GameState.Playing;
        canMove = (enabled && state == GameState.Playing) || state == GameState.BeingGuided;

        if (!canMove)
        {
            moveInput = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
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
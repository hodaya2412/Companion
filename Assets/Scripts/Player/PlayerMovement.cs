using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public Transform cam;

    private InputActions inputAction;
    private Rigidbody rb;
    private bool canMove = true;
    private Vector3 moveInput;

    private GameplayState currentGameplayState;
    private UIState currentUIState;

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

        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged += HandleUIStateChanged;
        GameEvents.OnDialogueStarted += HandleDialogueStarted;
        GameEvents.OnDialogueEnded += HandleDialogueEnded;

        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        currentUIState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        RefreshMovementState();
    }

    void OnDisable()
    {
        if (inputAction != null)
        {
            inputAction.Player.Move.performed -= OnMove;
            inputAction.Player.Move.canceled -= OnMove;
            inputAction.Player.Disable();
        }

        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnUIStateChanged -= HandleUIStateChanged;
        GameEvents.OnDialogueStarted -= HandleDialogueStarted;
        GameEvents.OnDialogueEnded -= HandleDialogueEnded;
    }

    private void HandleGameplayStateChanged(GameplayState state)
    {
        currentGameplayState = state;
        RefreshMovementState();
    }

    private void HandleUIStateChanged(UIState state)
    {
        currentUIState = state;
        RefreshMovementState();
    }

    private void HandleDialogueStarted()
    {
        currentUIState = UIState.Dialogue;
        RefreshMovementState();
    }

    private void HandleDialogueEnded()
    {
        currentUIState = UIState.None;
        RefreshMovementState();
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector3>();
    }

    private void RefreshMovementState()
    {
        bool gameplayAllowsMove =
            currentGameplayState == GameplayState.Playing ||
            currentGameplayState == GameplayState.Combat ||
            currentGameplayState == GameplayState.BeingGuided;

        bool uiBlocksMove =
            currentUIState == UIState.Dialogue ||
            currentUIState == UIState.Map ||
            currentUIState == UIState.Choice;

        canMove = gameplayAllowsMove && !uiBlocksMove;

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
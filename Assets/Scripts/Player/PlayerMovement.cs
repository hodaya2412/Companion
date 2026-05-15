using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public Transform cam;

    [Header("Visuals")]
    [Tooltip("גררי לכאן את האובייקט הילד שמכיל את ה-Sprite")]
    public Transform characterVisuals;
  

    private InputActions inputAction;
    private Rigidbody rb;
    private bool canMove = true;
    private Vector3 moveInput;

    private GameplayState currentGameplayState;
    private UIState currentUIState;

    [Header("Animation")]
    public Animator animator;

    [Header("Data Persistence")]
    public PlayerStateSO playerState;


    private void UpdateAnimator()
    {
        if (animator == null) return;

        // 1. קריאה בלבד מה-SO - האנימטור פשוט עושה מה שה-SO אומר לו
        animator.SetBool("IsArmed", playerState.isArmed);
        animator.SetInteger("WeaponType", playerState.weaponType);

        // 2. מהירות
        float speedVal = new Vector2(moveInput.x, moveInput.z).magnitude;
        animator.SetFloat("Speed", speedVal);

        // 3. כיוון מבט (עדכון ה-SO רק בזמן תנועה)
        if (speedVal > 0.01f)
        {
            playerState.lastMoveX = moveInput.x;
            playerState.lastMoveY = moveInput.z;
        }

        // הזרקת הכיוון לאנימטור (תמיד, כדי שיזכור כיוון בעמידה)
        animator.SetFloat("MoveX", playerState.lastMoveX);
        animator.SetFloat("MoveY", playerState.lastMoveY);
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        inputAction = new InputActions();
        UpdateAnimator();

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

        // עדכון מצב ראשוני
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
        Vector3 input = ctx.ReadValue<Vector3>();

        // נותן עדיפות לכיוון האחרון / ציר אחד בלבד
        if (Mathf.Abs(input.x) > Mathf.Abs(input.z))
        {
            moveInput = new Vector3(Mathf.Sign(input.x), 0, 0);
        }
        else if (Mathf.Abs(input.z) > 0)
        {
            moveInput = new Vector3(0, 0, Mathf.Sign(input.z));
        }
        else
        {
            moveInput = Vector3.zero;
        }
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
    currentUIState == UIState.Choice ||
    currentUIState == UIState.Inventory;

        canMove = gameplayAllowsMove && !uiBlocksMove;

        if (!canMove)
        {
            moveInput = Vector3.zero;
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
    }
    void Update()
    {

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (!canMove || (animator != null && animator.GetBool("IsAttacking")))
        {
            // איפוס המהירות הפיזית כדי שהדמות לא "תחליק" בזמן המכה
            if (rb != null) rb.linearVelocity = Vector3.zero;
            return;
        }

        // חישוב כיוון המצלמה
        Vector3 camForward = cam ? cam.forward : Vector3.forward;
        Vector3 camRight = cam ? cam.right : Vector3.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // כיוון תנועה סופי
        Vector3 moveDir = (camRight * moveInput.x + camForward * moveInput.z).normalized;
      

        // תנועה פיזית
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

        // טיפול בויזואליות (Flip)
        //ApplyVisualFlip(moveInput.x);
    }
}
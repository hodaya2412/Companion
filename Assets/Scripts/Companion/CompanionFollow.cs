using UnityEngine;


public class CompanionFollow : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb;

    [Header("Animation Settings")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Normal Follow")]
    public float comfortableDistance = 1.5f;
    public float maxDistance = 4.5f;
    public float maxSpeed = 8f;

    [Header("Combat Wait Position")]
    public float combatBackOffset = 2.5f;
    public float combatSideOffset = 2f;
    public float combatMoveSpeed = 6f;
    public float combatArrivalDistance = 0.2f;

    [SerializeField] private float zeroVectorMagnitudeThreshold = 0.05f;
    [SerializeField] private float animationDirectionIdle = 0f;
    [SerializeField] private float animationDirectionRight = 4f;
    [SerializeField] private float animationDirectionLeft = 3f;
    [SerializeField] private float animationDirectionUp = 2f;
    [SerializeField] private float animationDirectionDown = 1f;
    [SerializeField] private float forwardSqrMagnitudeThreshold = 0.001f;
    [SerializeField] private float sideRandomThreshold = 0.5f;
    [SerializeField] private float sideFactorNegative = -1f;
    [SerializeField] private float sideFactorPositive = 1f;

    private bool followEnabled = true;
    private GameplayState currentGameplayState = GameplayState.Playing;
    private Vector3 combatWaitPosition;
    private bool hasCombatWaitPosition = false;

    private void OnEnable()
    {
        Debug.Log("[CompanionFollow] OnEnable called");
        GameEvents.OnCompanionFollowEnabled += SetFollowEnabled;
        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        
        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;

    }

    private void OnDisable()
    {
        GameEvents.OnCompanionFollowEnabled -= SetFollowEnabled;
        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        
    }

    private void SetFollowEnabled(bool enabled)
    {
        followEnabled = enabled;
        Debug.Log("[CompanionFollow] followEnabled = " + enabled);
    }
    private void HandleGameplayStateChanged(GameplayState state)
    {
        currentGameplayState = state;
        if (state == GameplayState.Combat)
        {
            combatWaitPosition = CalculateCombatWaitPosition();
            hasCombatWaitPosition = true;
        }
        else hasCombatWaitPosition = false;
    }

    private void FixedUpdate()
    {
       
        if (player == null || rb == null) return;

        Vector3 moveDirection = Vector3.zero;

        if (followEnabled)
        {
            if (currentGameplayState == GameplayState.Combat)
                moveDirection = MoveToCombatWaitPosition();
            else
                moveDirection = FollowNormally();
        }

        UpdateSpriteAnimation(moveDirection);
    }

    private Vector3 FollowNormally()
    {
        Vector3 toPlayer = player.position - rb.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        if (distance <= comfortableDistance) return Vector3.zero;

        float t = Mathf.InverseLerp(comfortableDistance, maxDistance, distance);
        float speed = Mathf.Lerp(0f, maxSpeed, t);

        Vector3 move = toPlayer.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
        return toPlayer.normalized; 
    }

    private Vector3 MoveToCombatWaitPosition()
    {
        if (!hasCombatWaitPosition)
        {
            combatWaitPosition = CalculateCombatWaitPosition();
            hasCombatWaitPosition = true;
        }

        Vector3 toTarget = combatWaitPosition - rb.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;

        if (distance <= combatArrivalDistance) return Vector3.zero;

        Vector3 move = toTarget.normalized * combatMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
        return toTarget.normalized;
    }

   
    public void UpdateSpriteAnimation(Vector3 moveDir)
    {
        if (animator == null || spriteRenderer == null) return;

        if (moveDir.magnitude < zeroVectorMagnitudeThreshold)
        {
            animator.SetInteger("Direction", (int)animationDirectionIdle); 
            return;
        }

        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z))
        {
            if (moveDir.x > 0)
            {
                animator.SetInteger("Direction", (int)animationDirectionRight); 
                spriteRenderer.flipX = true;
            }
            else
            {
                animator.SetInteger("Direction", (int)animationDirectionLeft); 
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            spriteRenderer.flipX = false;
            if (moveDir.z > 0) animator.SetInteger("Direction", (int)animationDirectionUp); 
            else animator.SetInteger("Direction", (int)animationDirectionDown);
        }
    }

    private Vector3 CalculateCombatWaitPosition()
    {
        if (player == null)
        {
            return rb != null ? rb.position : Vector3.zero;

        }
        Vector3 playerForward = player.forward;
        playerForward.y = 0f;
        if (playerForward.sqrMagnitude < forwardSqrMagnitudeThreshold) playerForward = Vector3.forward;
        playerForward.Normalize();
        Vector3 side = Vector3.Cross(Vector3.up, playerForward).normalized;
        float sideSign = Random.value < sideRandomThreshold ? sideFactorNegative : sideFactorPositive;
        Vector3 target = player.position - playerForward * combatBackOffset + side * combatSideOffset * sideSign;
        target.y = rb.position.y;
        return target;
    }

   
}
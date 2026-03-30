using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public Transform player;
    public Rigidbody rb; // isKinematic = true

    [Header("Normal Follow")]
    public float comfortableDistance = 1.5f;
    public float maxDistance = 4.5f;
    public float maxSpeed = 8f;

    [Header("Combat Wait Position")]
    public float combatBackOffset = 2.5f;
    public float combatSideOffset = 2f;
    public float combatMoveSpeed = 6f;
    public float combatArrivalDistance = 0.2f;

    private bool followEnabled = true;
    private GameplayState currentGameplayState = GameplayState.Playing;

    private Vector3 combatWaitPosition;
    private bool hasCombatWaitPosition = false;

    private void OnEnable()
    {
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
    }

    private void HandleGameplayStateChanged(GameplayState state)
    {
        currentGameplayState = state;

        if (state == GameplayState.Combat)
        {
            combatWaitPosition = CalculateCombatWaitPosition();
            hasCombatWaitPosition = true;
        }
        else
        {
            hasCombatWaitPosition = false;
        }
    }

    private void FixedUpdate()
    {
        if (!followEnabled) return;
        if (player == null || rb == null) return;

        if (currentGameplayState == GameplayState.Combat)
            MoveToCombatWaitPosition();
        else
            FollowNormally();
    }

    private void FollowNormally()
    {
        Vector3 toPlayer = player.position - rb.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (distance <= comfortableDistance)
            return;

        float t = Mathf.InverseLerp(comfortableDistance, maxDistance, distance);
        float speed = Mathf.Lerp(0f, maxSpeed, t);

        Vector3 move = toPlayer.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void MoveToCombatWaitPosition()
    {
        if (!hasCombatWaitPosition)
        {
            combatWaitPosition = CalculateCombatWaitPosition();
            hasCombatWaitPosition = true;
        }

        Vector3 toTarget = combatWaitPosition - rb.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        if (distance <= combatArrivalDistance)
            return;

        Vector3 move = toTarget.normalized * combatMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private Vector3 CalculateCombatWaitPosition()
    {
        Vector3 playerForward = player.forward;
        playerForward.y = 0f;

        if (playerForward.sqrMagnitude < 0.001f)
            playerForward = Vector3.forward;

        playerForward.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, playerForward).normalized;

        float sideSign = Random.value < 0.5f ? -1f : 1f;

        Vector3 target =
            player.position
            - playerForward * combatBackOffset
            + side * combatSideOffset * sideSign;

        target.y = rb.position.y;
        return target;
    }
}
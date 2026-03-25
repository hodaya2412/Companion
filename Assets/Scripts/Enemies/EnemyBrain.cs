using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private enum EnemyState
    {
        ReturningToCave,
        Holding,
        Chasing,
        Waiting,
        Attacking
    }

    [Header("References")]
    [SerializeField] private BanditCaveAgent movement;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyHealth health;

    [Header("Behavior")]
    [SerializeField] private float waitBeforeAttackMin = 0.3f;
    [SerializeField] private float waitBeforeAttackMax = 1.2f;

    [Header("Holding")]
    [SerializeField] private float holdDistance = 4.5f;
    [SerializeField] private float holdTolerance = 1f;
    [SerializeField] private float holdUpdateInterval = 0.5f;
    [SerializeField] private float holdSideStepDistance = 2f;

    [Header("Slot Logic")]
    [SerializeField] private float loseAttackSlotDistance = 8f;

    [Header("Type")]
    [SerializeField] private EnemyTypeData typeData;

    [Header("Chasing")]
    [SerializeField] private float chaseUpdateInterval = 0.5f;
    [SerializeField] private float attackPositionReachDistance = 0.75f;
    [SerializeField] private float playerAttackAllowance = 1.0f;

    private EnemyState currentState = EnemyState.ReturningToCave;
    private float waitTimer;
    private float holdTimer;
    private float chaseTimer;

    private Vector3 currentChaseTarget;

    private GameplayState currentGameplayState;
    private bool hasAttackSlot;
    private AttackRole currentAttackRole = AttackRole.None;

    private void Awake()
    {
        if (movement == null) movement = GetComponent<BanditCaveAgent>();
        if (combat == null) combat = GetComponent<EnemyCombat>();
        if (health == null) health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        GameEvents.OnCombatTriggered += HandleCombatTriggered;
        GameEvents.OnCombatReset += HandleCombatReset;

        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
    }

    private void OnDisable()
    {
        ReleaseAttackSlotIfNeeded();

        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnCombatTriggered -= HandleCombatTriggered;
        GameEvents.OnCombatReset -= HandleCombatReset;
    }

    private void Start()
    {
        SetState(EnemyState.ReturningToCave);
    }

    private void Update()
    {
        if (health == null) return;

        switch (currentState)
        {
            case EnemyState.ReturningToCave:
                UpdateReturningToCave();
                break;

            case EnemyState.Holding:
                UpdateHolding();
                break;

            case EnemyState.Chasing:
                UpdateChasing();
                break;

            case EnemyState.Waiting:
                UpdateWaiting();
                break;

            case EnemyState.Attacking:
                UpdateAttacking();
                break;
        }
    }

    private void HandleGameplayStateChanged(GameplayState newState)
    {
        currentGameplayState = newState;

        if (newState != GameplayState.Combat)
        {
            ReleaseAttackSlotIfNeeded();
            SetState(EnemyState.ReturningToCave);
        }
    }

    private void HandleCombatTriggered()
    {
        ReleaseAttackSlotIfNeeded();
        SetState(EnemyState.Holding);
    }

    private void HandleCombatReset()
    {
        ReleaseAttackSlotIfNeeded();
        SetState(EnemyState.ReturningToCave);
    }

    private void UpdateReturningToCave()
    {
        if (movement == null) return;

        if (!movement.IsReturningToCave)
            movement.ReturnToCave();

        if (movement.HasReachedCave())
            movement.StopMoving();
    }

    private void UpdateHolding()
    {
        if (movement == null || combat == null) return;
        if (currentGameplayState != GameplayState.Combat) return;

        holdTimer -= Time.deltaTime;

        if (holdTimer <= 0f)
        {
            holdTimer = holdUpdateInterval;

            MoveAroundPlayerAtHoldDistance();
            TryBecomeActive();
        }
    }

    private void TryBecomeActive()
    {
        if (EnemyAttackCoordinator.Instance == null)
        {
            ActivateChasing();
            return;
        }

        if (EnemyAttackCoordinator.Instance.TryReserveAttackSlot(this))
        {
            ActivateChasing();
        }
    }

    private void ActivateChasing()
    {
        hasAttackSlot = true;
        chaseTimer = 0f;

        if (EnemyAttackCoordinator.Instance != null)
            currentChaseTarget = EnemyAttackCoordinator.Instance.GetAttackPosition(this);
        else
            currentChaseTarget = movement.GetPlayerPosition();

        SetState(EnemyState.Chasing);
    }

    private void UpdateChasing()
    {
        if (movement == null || combat == null) return;
        if (currentGameplayState != GameplayState.Combat) return;

        if (hasAttackSlot && IsTooFarToKeepSlot())
        {
            ReleaseAttackSlotIfNeeded();
            SetState(EnemyState.Holding);
            return;
        }

        if (!hasAttackSlot)
        {
            SetState(EnemyState.Holding);
            return;
        }

        chaseTimer -= Time.deltaTime;

        if (chaseTimer <= 0f)
        {
            chaseTimer = chaseUpdateInterval;

            if (EnemyAttackCoordinator.Instance != null)
                currentChaseTarget = EnemyAttackCoordinator.Instance.GetAttackPosition(this);
            else
                currentChaseTarget = movement.GetPlayerPosition();
        }

        movement.MoveToPosition(currentChaseTarget, 1.0f);

        bool reachedAssignedPosition =
            movement.HasReachedPosition(currentChaseTarget, 1.0f);

        bool closeEnoughToAttackPlayer =
            movement.IsPlayerInRange(combat.attackRange + playerAttackAllowance);

        if (reachedAssignedPosition && closeEnoughToAttackPlayer)
        {
            waitTimer = Random.Range(waitBeforeAttackMin, waitBeforeAttackMax);
            SetState(EnemyState.Waiting);
        }
    }

    private void UpdateWaiting()
    {
        if (movement == null || combat == null) return;
        if (currentGameplayState != GameplayState.Combat) return;

        if (hasAttackSlot && IsTooFarToKeepSlot())
        {
            ReleaseAttackSlotIfNeeded();
            SetState(EnemyState.Holding);
            return;
        }

        if (!hasAttackSlot)
        {
            SetState(EnemyState.Holding);
            return;
        }

        waitTimer -= Time.deltaTime;

        bool reachedAssignedPosition =
            movement.HasReachedPosition(currentChaseTarget, attackPositionReachDistance);

        bool closeEnoughToAttackPlayer =
            movement.IsPlayerInRange(combat.attackRange + playerAttackAllowance);

        if (!reachedAssignedPosition || !closeEnoughToAttackPlayer)
        {
            SetState(EnemyState.Chasing);
            return;
        }

        movement.StopMoving();

        if (waitTimer <= 0f)
            SetState(EnemyState.Attacking);
    }

    private void UpdateAttacking()
    {
        if (movement == null || combat == null) return;
        if (currentGameplayState != GameplayState.Combat) return;

        if (hasAttackSlot && IsTooFarToKeepSlot())
        {
            ReleaseAttackSlotIfNeeded();
            SetState(EnemyState.Holding);
            return;
        }

        if (!hasAttackSlot)
        {
            SetState(EnemyState.Holding);
            return;
        }

        bool closeEnoughToAttackPlayer =
            movement.IsPlayerInRange(combat.attackRange + playerAttackAllowance);

        if (!closeEnoughToAttackPlayer)
        {
            SetState(EnemyState.Chasing);
            return;
        }

        combat.TryAttack();

        waitTimer = Random.Range(waitBeforeAttackMin, waitBeforeAttackMax);
        SetState(EnemyState.Waiting);
    }

    private void MoveAroundPlayerAtHoldDistance()
    {
        Vector3 playerPos = movement.GetPlayerPosition();
        Vector3 myPos = transform.position;

        Vector3 toEnemy = myPos - playerPos;
        toEnemy.y = 0f;

        if (toEnemy.sqrMagnitude < 0.01f)
            toEnemy = transform.right;

        float distance = toEnemy.magnitude;
        Vector3 dir = toEnemy.normalized;
        Vector3 targetPos;

        if (distance > holdDistance + holdTolerance || distance < holdDistance - holdTolerance)
        {
            targetPos = playerPos + dir * holdDistance;
        }
        else
        {
            Vector3 sideDir = Vector3.Cross(Vector3.up, dir).normalized;
            float randomSide = Random.value < 0.5f ? -1f : 1f;
            targetPos = myPos + sideDir * randomSide * holdSideStepDistance;
        }

        movement.MoveToPosition(targetPos, 0.8f);
    }

    private bool IsTooFarToKeepSlot()
    {
        Vector3 playerPos = movement.GetPlayerPosition();
        float dist = Vector3.Distance(transform.position, playerPos);
        return dist > loseAttackSlotDistance;
    }

    private void ReleaseAttackSlotIfNeeded()
    {
        if (!hasAttackSlot) return;

        if (EnemyAttackCoordinator.Instance != null)
            EnemyAttackCoordinator.Instance.ReleaseAttackSlot(this);

        hasAttackSlot = false;
        currentAttackRole = AttackRole.None;
    }

    private void SetState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Holding)
            holdTimer = 0f;
    }

    public float GetFlankDesire()
    {
        if (typeData != null)
            return typeData.flankDesire;

        return 0.5f;
    }

    public bool PrefersFlank()
    {
        if (typeData != null)
            return typeData.preferFlank;

        return false;
    }

    public void SetAttackRole(AttackRole role)
    {
        currentAttackRole = role;
    }

    public AttackRole GetAttackRole()
    {
        return currentAttackRole;
    }
}